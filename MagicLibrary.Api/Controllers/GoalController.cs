using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagicLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly RecommendationService _Rservice;
        private readonly GoalService _Gservice;
        private readonly BookService _Bservice;

        public GoalController(GoalService Gservice, RecommendationService Rservice, BookService Bservice)
        {
            _Gservice = Gservice;
            _Rservice = Rservice;
            _Bservice = Bservice;
        }

        // ¡Ya está encendido de nuevo!
        [HttpGet]
        public IActionResult GetAll() => Ok(_Gservice.ObtenerTodos());

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var meta = _Gservice.ObtenerPorId(Id);

            // Si es null regresamos 404, si sí tiene datos regresamos 200 OK
            return meta == null ? NotFound() : Ok(meta);
        }
        //para agregar el post para verificar despues de que esta parte funcione
        [HttpPost("confirmar/{Id}")]
        public IActionResult ConfirmarLibroAgregado(int Id)
        {
            var goal = _Gservice.ObtenerPorId(Id);
            if (goal == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningun libro con ese ID {Id}" });
            }

            _Gservice.ConfirmarLibroAgregado(goal);

            // Si es null regresamos 404, si sí tiene datos regresamos 200 OK
            return Ok(new { mensaje = "Libro agregado exitosamente", goal = goal });
        }
        [HttpPost]
        public IActionResult TestLibros()
        {

            var libros = _Bservice.ObtenerTodos();
            return Ok(libros);
        }

    }


}