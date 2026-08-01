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
            while (!stoppingToken.IsCancellationRequested)
            {
                var horaActual = DateTime.Now.ToString("HH:mm");

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var goalService = scope.ServiceProvider.GetRequiredService<IGoalService>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>(); // 👈 USAMOS IUserService

                        // 1. Obtener metas configuradas
                        var metas = goalService.ObtenerTodos() ?? new System.Collections.Generic.List<Domain.Models.Goal>();

                        // 2. Filtrar las metas que coincidan con la hora actual
                        var metasAEnviar = metas.Where(g => g.HoraNotificacion == horaActual).ToList();

                        foreach (var meta in metasAEnviar)
                        {
                            // 🔑 Buscamos al usuario correspondiente y leemos la propiedad "Correo"
                            var usuario = userService.ObtenerPorId(meta.IdUsuario);

                            if (usuario != null && !string.IsNullOrEmpty(usuario.Correo))
                            {
                                string asunto = "📚 ¡Hora de leer! - MagicLibrary";
                                string mensaje = $"<h2>¡Hola {usuario.Nombre}!</h2><p>Son las {horaActual} hrs. Es momento de avanzar en tu meta de lectura del año {meta.Anio}. ¡Abre tu libro!</p>";

                                await emailService.SendEmailAsync(usuario.Correo, asunto, mensaje);
                            }
                        }
                    }
                }
                catch
                {
                    // Evita interrupciones en la app si ocurre un fallo de red
                }

                // Espera 1 minuto antes de volver a verificar el reloj
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}