using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MagicLibrary.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MagicLibrary.Infrastructure.Services
{
    public class NotificacionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificacionBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Obtener la hora UTC actual del servidor
                    DateTime horaUtc = DateTime.UtcNow;

                    // 2. Obtener la Zona Horaria de México (Mérida / CST) compatible con Windows y Linux (AWS)
                    string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? "Central Standard Time"
                        : "America/Mexico_City";

                    TimeZoneInfo timeZoneMexico = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    DateTime horaLocalMexico = TimeZoneInfo.ConvertTimeFromUtc(horaUtc, timeZoneMexico);

                    string horaActual = horaLocalMexico.ToString("HH:mm");
                    var diaActual = horaLocalMexico.DayOfWeek;

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var goalService = scope.ServiceProvider.GetRequiredService<IGoalService>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                        var metas = goalService.ObtenerTodos() ?? new System.Collections.Generic.List<Domain.Models.Goal>();

                        // 1. Obtenemos la hora y minutos actuales en México
                        int horaActualInt = horaLocalMexico.Hour;     // Ejemplo: 14 (las 2 PM)
                        int minutoActualInt = horaLocalMexico.Minute; // Ejemplo: 15

                        // 2. Comparamos inteligentemente parseando el texto de la meta
                        var metasAEnviar = metas.Where(g =>
                        {
                            if (string.IsNullOrWhiteSpace(g.HoraNotificacion)) return false;

                            // DateTime.TryParse entiende automáticamente "02:15 PM", "14:15", "2:15 PM", etc.
                            if (DateTime.TryParse(g.HoraNotificacion, out DateTime horaMetaParsed))
                            {
                                return horaMetaParsed.Hour == horaActualInt && horaMetaParsed.Minute == minutoActualInt;
                            }

                            return false;
                        }).ToList();

                        foreach (var meta in metasAEnviar)
                        {
                            bool enviarHoy = meta.DiasPorSemana switch
                            {
                                1 => diaActual == DayOfWeek.Wednesday,
                                3 => diaActual == DayOfWeek.Monday || diaActual == DayOfWeek.Wednesday || diaActual == DayOfWeek.Friday,
                                5 => diaActual >= DayOfWeek.Monday && diaActual <= DayOfWeek.Friday,
                                _ => true
                            };

                            if (!enviarHoy) continue;

                            var usuario = userService.ObtenerPorId(meta.IdUsuario);

                            if (usuario != null && !string.IsNullOrEmpty(usuario.Correo))
                            {
                                string asunto = "¡Hora de leer! - MagicLibrary";
                                string mensaje = $"<h2>¡Hola {usuario.Nombre}!</h2><p>Son las {horaActual} hrs en tu zona horaria. Es momento de avanzar en tu meta de lectura. ¡Abre tu libro!</p>";

                                await emailService.SendEmailAsync(usuario.Correo, asunto, mensaje);
                            }
                        }
                    }
                }
                catch
                {
                    // Evita interrupciones en la app si ocurre un fallo
                }

                // Verificamos cada 30 segundos para asegurar que no se pase el minuto
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}