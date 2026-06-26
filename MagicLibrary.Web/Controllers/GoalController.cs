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
            var diasRestantes = _Gservice.CalcularDiasRestantesAnio();
            if (diasRestantes <= 0) diasRestantes = 1; 

            int totalPaginas = 0;
            if (metaActual.LibrosAsignados != null)
            {
                foreach (var item in metaActual.LibrosAsignados)
                {
                    if (!item.EstaCompletado && item.RecomendacionId.HasValue)
                    {
                        var detallesLibro = _Rservice.ObtenerPorId(item.RecomendacionId.Value);
                        if (detallesLibro != null)
                        {
                            totalPaginas += detallesLibro.Paginas;
                        }
                    }
                }

            }
            //pasar los datos a la vista
            ViewBag.DiasRestantes = diasRestantes;
            ViewBag.TotalPaginas = totalPaginas; //mandar suma
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
                // si el json no inicializa la lista la creamoa aqui para que no explote

                if (metaActual.LibrosAsignados == null)
                {
                    metaActual.LibrosAsignados = new List<GoalItem>();
                }

                //agregar a la lista de la meta y guardamos los cambios

                metaActual.LibrosAsignados.Add(nuevoItem);
                _Gservice.Actualizar(metaActual);
            }
            // RECARGAR LA PAGINA PARA QUE SE VEA LA NUEVA LINEA
            return RedirectToAction("Index");
         }
        [HttpPost]
        public IActionResult ActualizarMeta(int nuevaCantidad)
        {
            // buscamos la meta actual
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(1, DateTime.Now.Year);

            //Actualizar la cantidad y guarda
            metaActual.CantidadObjetivo = nuevaCantidad;
            _Gservice.Actualizar(metaActual);
            //recargar pagina

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarcarCompletado(string tituloLibro)
        { 
            // buscamos la meta actual
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(1, DateTime.Now.Year);
            //buscar el libro especifico dentro de la libreta usando LINQ
            var libro = metaActual.LibrosAsignados.FirstOrDefault(i => i.Titulo == tituloLibro);
            
            if (libro != null)
            {
                //ponemos la palomita y guardamos
                libro.EstaCompletado = true;
                _Gservice.Actualizar(metaActual);
            }

           //recargar pagina
            return RedirectToAction("Index");
        }

    }
}

