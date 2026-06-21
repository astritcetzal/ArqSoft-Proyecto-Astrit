using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace MagicLibrary.Web.Controllers
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
