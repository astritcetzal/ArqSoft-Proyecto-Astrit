using System;
using System.Linq;
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
            var horaActual = DateTime.Now.ToString("HH:mm");
            var diaActual = DateTime.Today.DayOfWeek; // Obtenemos qué día es hoy

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var goalService = scope.ServiceProvider.GetRequiredService<IGoalService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var metas = goalService.ObtenerTodos() ?? new System.Collections.Generic.List<Domain.Models.Goal>();
                    var metasAEnviar = metas.Where(g => g.HoraNotificacion == horaActual).ToList();

                    foreach (var meta in metasAEnviar)
                    {
                        // --- LÓGICA DE DÍAS DE LA SEMANA ---
                        // Decidimos si hoy le toca notificación según la frecuencia que eligió
                        bool enviarHoy = meta.DiasPorSemana switch
                        {
                            1 => diaActual == DayOfWeek.Wednesday, // 1 día: Solo los Miércoles
                            3 => diaActual == DayOfWeek.Monday || diaActual == DayOfWeek.Wednesday || diaActual == DayOfWeek.Friday, // 3 días: Lunes, Miércoles, Viernes
                            5 => diaActual >= DayOfWeek.Monday && diaActual <= DayOfWeek.Friday, // 5 días: Lunes a Viernes
                            _ => true // Si puso 7 días, se envía diario
                        };

                        // Si hoy NO le toca notificación, saltamos a la siguiente meta
                        if (!enviarHoy) continue;

                        var usuario = userService.ObtenerPorId(meta.IdUsuario);

                        if (usuario != null && !string.IsNullOrEmpty(usuario.Correo))
                        {
                            string asunto = " ¡Hora de leer! - MagicLibrary";
                            string mensaje = $"<h2>¡Hola {usuario.Nombre}!</h2><p>Son las {horaActual} hrs. Hoy toca avanzar en tu meta de lectura del año {meta.Anio}. ¡Abre tu libro!</p>";

                            await emailService.SendEmailAsync(usuario.Correo, asunto, mensaje);
                        }
                    }
                }
            }
            catch
            {
                // Evita interrupciones en la app si ocurre un fallo de red
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}