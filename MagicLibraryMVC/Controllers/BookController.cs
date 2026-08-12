using MagicLibraryMVC.Interfaces;
using MagicLibraryMVC.Models;
using MagicLibraryMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace MagicLibraryMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _service;
        public BookController(BookService service)
        {
            _service = service;
        }

        //filtrado
        public IActionResult Index(string? estado)
        {
            var libros = string.IsNullOrEmpty(estado)
                ? _service.ObtenerTodos()
                : _service.ObtenerPorTipoEstado(estado);
            ViewBag.Estados = _service.ObtenerTipoEstado();
            ViewBag.EstadoAcual = estado;
            return View(libros);
        }

        //detalle
        public IActionResult Detalle(int id)
        {
            var libro = _service.ObtenerPorId(id);

          

            return View(libro);
        }
        // formulario get
        public IActionResult Agregar()
        {
            return View();
        }

        //formulario - post
        [HttpPost]
        public IActionResult Agregar(Book libro)
        {
            _service.Agregar(libro);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GuardarMiOpinion(int idLibro, int calificacion, string comentario)
        {
            // 1. Buscamos el libro en el JSON
            var libro = _service.ObtenerPorId(idLibro);

            if (libro != null)
            {
                // 2. Le inyectamos tu calificación y reseña personal
                libro.CalificacionPersonal = calificacion;
                libro.ResenaPersonal = comentario;

                // 3. Guardamos los cambios
                _service.Actualizar(libro);
            }

            // Recargamos la página de detalles
            return RedirectToAction("Detalle", new { id = idLibro });
        }
        // GET: Muestra la pantalla para editar la opinión
        public IActionResult EditarResena(int idLibro)
        {
            var libro = _service.ObtenerPorId(idLibro);
            if (libro == null) return NotFound();

            return View(libro); // Te manda a la vista que acabamos de arreglar
        }
    }
}
