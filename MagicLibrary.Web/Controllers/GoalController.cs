using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class GoalController : Controller
    {
        private readonly IGoalService _Gservice;
        private readonly IRecommendationService _Rservice;
        private readonly IBookService _Bservice;
        private readonly IEmailService _emailService;

        public GoalController(
            IGoalService Gservice,
            IRecommendationService Rservice,
            IBookService Bservice,
            IEmailService emailService)
        {
            _Gservice = Gservice;
            _Rservice = Rservice;
            _Bservice = Bservice;
            _emailService = emailService;
        }

        private int ObtenerUserIdEnSesion()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
        }

        public IActionResult Index()
        {
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("Welcome", "Home");

            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(userId, DateTime.Now.Year);

            int diasRestantes = _Gservice.CalcularDiasRestantesAnio(metaActual.DiasPorSemana);
            int totalPaginas = _Gservice.CalcularTotalPaginasPendientes(metaActual);

            ViewBag.DiasRestantes = diasRestantes;
            ViewBag.TotalPaginas = totalPaginas;

            // 🔑 EL .ToList() EVITA QUE EL CASTEO EN RAZOR DEVUELVA NULL
            ViewBag.Recommendation = _Rservice.ObtenerTodos()?.ToList() ?? new List<Recommendation>();
            ViewBag.MisLibros = _Bservice.ObtenerTodos()
                                        .Where(b => b.UserId == userId)
                                        .ToList();

            return View(metaActual);
        }

        [HttpPost]
        public IActionResult AgregarItem(int? RecomendacionId, int? MiLibroId)
        {
            int userId = ObtenerUserIdEnSesion();
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(userId, DateTime.Now.Year);
            if (metaActual.LibrosAsignados == null) metaActual.LibrosAsignados = new List<GoalItem>();

            if (MiLibroId.HasValue && MiLibroId.Value > 0)
            {
                var miLibro = _Bservice.ObtenerPorId(MiLibroId.Value);
                if (miLibro != null)
                {
                    metaActual.LibrosAsignados.Add(new GoalItem
                    {
                        Titulo = miLibro.Titulo,
                        MiLibroId = miLibro.IdLibro,
                        EstaCompletado = false
                    });
                }
            }
            else if (RecomendacionId.HasValue && RecomendacionId.Value > 0)
            {
                var recomendacion = _Rservice.ObtenerPorId(RecomendacionId.Value);
                if (recomendacion != null)
                {
                    metaActual.LibrosAsignados.Add(new GoalItem
                    {
                        Titulo = recomendacion.TituloLibro,
                        RecomendacionId = recomendacion.Id,
                        EstaCompletado = false
                    });
                }
            }

            _Gservice.Actualizar(metaActual);
            _Gservice.ConfirmarLibroAgregado(metaActual);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ConfigurarMeta(int nuevaCantidad, int diasPorSemana, string horaNotificacion)
        {
            int userId = ObtenerUserIdEnSesion();
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(userId, DateTime.Now.Year);

            metaActual.CantidadObjetivo = nuevaCantidad;
            metaActual.DiasPorSemana = diasPorSemana;
            metaActual.HoraNotificacion = horaNotificacion;

            _Gservice.Actualizar(metaActual);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarcarCompletado(string tituloLibro)
        {
            int userId = ObtenerUserIdEnSesion();
            _Gservice.CompletarLibroEnMeta(userId, DateTime.Now.Year, tituloLibro);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EnviarRecordatorioPrueba()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;

            if (!string.IsNullOrEmpty(userEmail))
            {
                string asunto = "Recordatorio de Lectura - MagicLibrary";
                string mensaje = "<h2 style='color:#311b58;'>¡Es hora de leer!</h2><p>Este es un correo de prueba para confirmar tus notificaciones de metas de lectura.</p>";
                await _emailService.SendEmailAsync(userEmail, asunto, mensaje);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EliminarItem(string tituloLibro)
        {
            int userId = ObtenerUserIdEnSesion();
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(userId, DateTime.Now.Year);

            if (metaActual.LibrosAsignados != null)
            {
                // Busca la primera coincidencia del título y la remueve de la lista
                var itemAEliminar = metaActual.LibrosAsignados.FirstOrDefault(i => i.Titulo == tituloLibro);
                if (itemAEliminar != null)
                {
                    metaActual.LibrosAsignados.Remove(itemAEliminar);
                    _Gservice.Actualizar(metaActual);
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ConfirmarLecturaHoy()
        {
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("PrincipalInicio", "Home");

            // Ejecutamos la regla de negocio
            _Gservice.ConfirmarLecturaDiaria(userId, DateTime.Now.Year);

            // Guardamos un mensaje temporal para la vista
            TempData["MensajeConfirmacion"] = "¡Excelente trabajo! Has registrado tu lectura de hoy.";

            return RedirectToAction("Index");
        }
    }
}