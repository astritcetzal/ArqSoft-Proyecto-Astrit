using MagicLibraryMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagicLibraryMVC.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly RecommendationService _service;
        public RecommendationController(RecommendationService service)
        {
            _service = service;
        }
        public IActionResult Index(string? genero)
        {
            var recomendaciones = string.IsNullOrEmpty(genero)
                ? _service.ObtenerTodos()
                : _service.ObtenerPorGenero(genero);
            ViewBag.Generos = _service.ObtenerGenero();
            ViewBag.GeneroActual = genero;
            return View(recomendaciones);
        }

        public IActionResult Detalle(int id)
        {
            var recomendacion = _service.ObtenerPorId(id);
            return View(recomendacion);
        }
    }
}
