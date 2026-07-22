using MagicLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class RecommendationController : Controller
    {
        private readonly IRecommendationService _service;
        public RecommendationController(IRecommendationService service)
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
