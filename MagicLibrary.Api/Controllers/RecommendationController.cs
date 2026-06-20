using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagicLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _service;

        public RecommendationController(RecommendationService service)
        {
            _service = service;
        }


        [HttpGet]
        public IActionResult GetAll() => Ok(_service.ObtenerTodos());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var recomendacion = _service.ObtenerPorId(id);
            return recomendacion == null ? NotFound($"No se encontraron libros con el id: {id}") : Ok(recomendacion);
        }

        [HttpGet("filter/genre")]
        public IActionResult GetByGenre([FromQuery] string genero)
        {
            // Lógica para filtrar
            var recomendaciones = _service.ObtenerTodos()
                .Where(r => r.genero.Equals(genero, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return recomendaciones.Any() ? Ok(recomendaciones) : NotFound($"No hay recomendaciones para el género: {genero}");
        }
    }
}
