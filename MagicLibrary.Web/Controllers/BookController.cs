using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _service;
        private readonly IRecommendationService _recService;
        private readonly IAiService _aiService;
        public BookController(IBookService service, IRecommendationService recService, IAiService aiService)
        {
            _service = service;
            _recService = recService;
            _aiService = aiService;
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
            libro.UserId = userId; // Le asignamos el dueño

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
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("IniciarSesion", "User");

            var rec = _recService.ObtenerPorId(id);
            if (rec != null)
            {
                var nuevoLibro = new Book
                {
                    UserId = userId,
                    Titulo = rec.TituloLibro,
                    Autor = rec.Autor,
                    Paginas = rec.Paginas,
                    Estado = "Pendiente",
                    FechaInicio = DateOnly.FromDateTime(DateTime.Now)
                };

                _service.Agregar(nuevoLibro);
            }

            return RedirectToAction("Index");
        }

        // 3. Método para importación masiva de libros mediante IA
        [HttpPost]
        public async Task<IActionResult> CargarMasivoIA(string promptLibros)
        {
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("Welcome", "Home");

            if (!string.IsNullOrWhiteSpace(promptLibros))
            {
                var librosExtraidos = await _aiService.ExtraerLibrosDeTextoAsync(promptLibros);

                foreach (var libro in librosExtraidos)
                {
                    libro.UserId = userId;
                    _service.Agregar(libro); // Persiste cada libro extraído en la BD
                }
            }

            return RedirectToAction("Index");
        }
        // GET: Carga el formulario de edición
        [HttpGet]
        public IActionResult EditarDetalles(int id)
        {
            var libro = _service.ObtenerPorId(id);
            if (libro == null) return NotFound();

            ViewBag.Estados = _service.ObtenerTipoEstado();
            return View("EditarDetalles", libro);
        }

        // POST: Recibe los datos guardados y actualiza la BD (RESUELVE EL ERROR 405)
        [HttpPost]
        public IActionResult EditarDetalles(Book libro)
        {
            int userId = ObtenerUserIdEnSesion();
            libro.UserId = userId; // Conserva el dueño del libro

            _service.Actualizar(libro);
            return RedirectToAction("Detalle", new { id = libro.IdLibro });
        }
    }
}