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
        private readonly BookService _Bservice;

        public GoalController(GoalService Gservice,RecommendationService Rservice, BookService Bservice)
        {
            _Gservice = Gservice;
            _Rservice = Rservice;
            _Bservice = Bservice;
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
                    // Sumamos también si viene de Mis Libros
                    else if (!item.EstaCompletado && item.MiLibroId.HasValue)
                    {
                        var detallesLibro = _Bservice.ObtenerPorId(item.MiLibroId.Value);
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
            // 2. Mandamos a la vista los libros que el usuario está leyendo o por leer
            ViewBag.MisLibros = _Bservice.ObtenerTodos().Where(b => b.Estado != "Terminado").ToList();
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
        public IActionResult AgregarItem(int? RecomendacionId, int? MiLibroId)
        {
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(1, DateTime.Now.Year);
            if (metaActual.LibrosAsignados == null) metaActual.LibrosAsignados = new List<GoalItem>();

            // 3A. Si el usuario eligió de Recomendaciones
            if (RecomendacionId.HasValue)
            {
                var recomendacion = _Rservice.ObtenerPorId(RecomendacionId.Value);
                if (recomendacion != null)
                {
                    metaActual.LibrosAsignados.Add(new GoalItem
                    {
                        Titulo = recomendacion.TituloLibro,
                        RecomendacionId = recomendacion.Id,
                        EstaCompletado = false
                    });
                }
            }
            // 3B. Si el usuario eligió de Mis Libros
            else if (MiLibroId.HasValue)
            {
                var miLibro = _Bservice.ObtenerPorId(MiLibroId.Value);
                if (miLibro != null)
                {
                    metaActual.LibrosAsignados.Add(new GoalItem
                    {
                        Titulo = miLibro.Titulo,
                        MiLibroId = miLibro.IdLibro,
                        EstaCompletado = false
                    });
                }
            }

            _Gservice.Actualizar(metaActual);
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
            var metaActual = _Gservice.ObtenerMetaOCrearPorDefecto(1, DateTime.Now.Year);
            var libroEnMeta = metaActual.LibrosAsignados.FirstOrDefault(i => i.Titulo == tituloLibro);

            if (libroEnMeta != null)
            {
                // 4. Marcamos la palomita en la libreta
                libroEnMeta.EstaCompletado = true;
                _Gservice.Actualizar(metaActual);

                // 5. Lógica Hexagonal: Actualizar el estado en el inventario real
                if (libroEnMeta.MiLibroId.HasValue)
                {
                    // Si ya era un libro nuestro, lo actualizamos
                    var libroReal = _Bservice.ObtenerPorId(libroEnMeta.MiLibroId.Value);
                    if (libroReal != null)
                    {
                        libroReal.Estado = "Terminado";
                        _Bservice.Actualizar(libroReal);
                    }
                }
                else if (libroEnMeta.RecomendacionId.HasValue)
                {
                    // Si era una recomendación, lo agregamos a Mis Libros como terminado
                    var recomendacion = _Rservice.ObtenerPorId(libroEnMeta.RecomendacionId.Value);
                    if (recomendacion != null)
                    {
                        var nuevoLibro = _Bservice.PrepararLibroDesdeRecomendacion(recomendacion);
                        nuevoLibro.Estado = "Terminado";
                        _Bservice.Agregar(nuevoLibro);
                    }
                }
            }

            return RedirectToAction("Index");
        }

    }
}

