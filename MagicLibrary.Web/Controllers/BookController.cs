using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _service;
        private readonly IRecommendationService _recService;

        public BookController(IBookService service, IRecommendationService recService)
        {
            _service = service;
            _recService = recService;
        }

        // Método auxiliar para obtener el ID del usuario en sesión fácilmente
        private int ObtenerUserIdEnSesion()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
        }

        // 1. Un solo Index que maneja tanto el usuario como el filtro de estado
        public IActionResult Index(string? estado)
        {
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("Welcome", "Home");

            // Obtenemos SOLO los libros del usuario que inició sesión
            var misLibros = _service.ObtenerTodos()
                                    .Where(b => b.UserId == userId);

            // Si filtró por estado (Pendiente, Leyendo, etc.), aplicamos el filtro adicional
            if (!string.IsNullOrEmpty(estado))
            {
                misLibros = misLibros.Where(b => b.Estado == estado);
            }

            ViewBag.Estados = _service.ObtenerTipoEstado();
            ViewBag.EstadoAcual = estado;

            return View(misLibros.ToList());
        }

        public IActionResult Detalle(int id)
        {
            var libro = _service.ObtenerPorId(id);
            return View(libro);
        }

        public IActionResult Agregar()
        {
            return View();
        }

        // 2. Al crear un libro, le estampamos el ID del usuario
        [HttpPost]
        public IActionResult Agregar(Book libro)
        {
            int userId = ObtenerUserIdEnSesion();
            libro.UserId = userId; //Le asignamos el dueño

            _service.Agregar(libro);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GuardarMiOpinion(int idLibro, int calificacion, string comentario)
        {
            var libro = _service.ObtenerPorId(idLibro);
            if (libro != null)
            {
                libro.CalificacionPersonal = calificacion;
                libro.ResenaPersonal = comentario;
                _service.Actualizar(libro);
            }
            return RedirectToAction("Detalle", new { id = idLibro });
        }

        public IActionResult EditarResena(int idLibro)
        {
            var libro = _service.ObtenerPorId(idLibro);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [HttpGet]
        public IActionResult AgregarDesdeRecomendacion(int id)
        {
            var recomendacion = _recService.ObtenerPorId(id);
            if (recomendacion == null) return NotFound();

            var nuevoLibro = _service.PrepararLibroDesdeRecomendacion(recomendacion);
            return View("Agregar", nuevoLibro);
        }
    }
}