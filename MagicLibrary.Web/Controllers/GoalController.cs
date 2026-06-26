using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreGeneratedDocument;

using Microsoft.AspNetCore.Authorization; //para login 
namespace MagicLibrary.Web.Controllers
{
    [Authorize] //ára que sea obligatoria la autenticacion 
    public class GoalController : Controller
    {
        private readonly RecommendationService _Rservice;
        private readonly GoalService _Gservice;

        public GoalController(GoalService Gservice,RecommendationService Rservice)
        {
            _Gservice = Gservice;
            _Rservice = Rservice;
        }


        public IActionResult Index()
        {
            ViewBag.Recommendation = _Rservice.ObtenerTodos();

            //Pedir datos procesados a los servicios
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(1, DateTime.Now.Year);

            //pasar los datos a la vista
            ViewBag.DiasRestantes = _Gservice.CalcularDiasRestantesAnio();
            ViewBag.Recommendation = _Rservice.ObtenerTodos();

            return View(metaActual);
        }

        
        public IActionResult ObtenerPorId(int id)
        {
            var goals = _Gservice.ObtenerTodos().Where(g=> g.IdMeta ==id );
            ViewBag.Recommendation = _Rservice.ObtenerTodos();
            return View(goals);
        }
        // POST
        [HttpPost]
        public IActionResult Agregar(Goal goals)
        {
            ViewBag.Recommendation = _Rservice.ObtenerTodos();
            _Gservice.Agregar(goals);
            return RedirectToAction("Index");
        }

        public IActionResult Actualizar(Goal goals)
        {
            return View(goals);
        }

        [HttpPost]
        public IActionResult AgregarItem(int RecomendacionId)
        {
            //buscar meta actal del usaurio
            var metaActual = _Gservice.ObtenerMetaActual(1, DateTime.Now.Year);
            if (metaActual == null) return RedirectToAction("Index");
            // buscar los detalless de la recomendacion que le usuario seleccionó
            var recomendacion = _Rservice.ObtenerPorId(RecomendacionId);
            if (recomendacion != null)
            {

                //crear nuevo item para la libreta
                var nuevoItem = new GoalItem
                {
                    Titulo = recomendacion.TituloLibro,
                    RecomendacionId = recomendacion.Id,
                    EstaCompletado = false
                };
                //agregar a la lista de la meta y guardamos los cambios
                metaActual.LibrosAsignados.Add(nuevoItem);
                _Gservice.Actualizar(metaActual);
            }
            // RECARGAR LA PAGINA PARA QUE SE VEA LA NUEVA LINEA
            return RedirectToAction("Index");
         }
    }
}

