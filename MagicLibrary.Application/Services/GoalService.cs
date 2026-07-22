using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;

namespace MagicLibrary.Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _repo;
        private readonly IEnumerable<IGoalObserver> _observers;
        private readonly IBookService _bservice;
        private readonly IRecommendationService _rService;
        public GoalService(IGoalRepository repo, IEnumerable<IGoalObserver> observers, IBookService bservice, IRecommendationService rService)

        {
            _repo = repo;
            _observers = observers;
            _bservice = bservice;
            _rService = rService;
        }
        public Goal ObtenerMetaOCrearPorDefecto(int idUsuario, int anio)
        {
            var meta = _repo.ObtenerTodos().FirstOrDefault(g => g.IdUsuario == idUsuario && g.Anio == anio);
            if (meta == null)
            {
                meta = new Goal
                {
                    IdUsuario = idUsuario,
                    Anio = anio,
                    CantidadObjetivo = 5,
                    LibrosAsignados = new List<GoalItem>()
                };
                _repo.Agregar(meta);
            }
            return meta;
        }
        // lógica para calculos matematicos del negocio
        public int CalcularDiasRestantesAnio()
        {
            DateTime finDeAnio = new DateTime(DateTime.Now.Year, 12, 31);
            return (finDeAnio - DateTime.Now).Days;
        }
        public List<Goal> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
        public Goal? ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }
        public void Agregar(Goal goal)
        {
            //validaciones de negoco agregar para que no trunque el sistema
            _repo.Agregar(goal);
        }
        public void Actualizar(Goal goal)
        {
            _repo.Actualizar(goal);
        }
        public Goal? ObtenerMetaActual(int idUsuario, int anio)
        {
            return _repo.ObtenerTodos().FirstOrDefault(g => g.IdUsuario == idUsuario && g.Anio == anio);
        }
        public void ConfirmarLibroAgregado(Goal goal)
        {//notificar 
            foreach (var observer in _observers)
            {
                observer.OnSavedBook(goal);
            }
        } 
        public int CalcularTotalPaginasPendientes(Goal metaActual)
        {
            int totalPaginas = 0;
            if (metaActual.LibrosAsignados != null) 
            { 
                foreach (var item in metaActual.LibrosAsignados)
                {
                    if (!item.EstaCompletado && item.RecomendacionId.HasValue)
                    {
                        var detallesLibro = _rService.ObtenerPorId(item.RecomendacionId.Value);
                        if (detallesLibro != null) totalPaginas += detallesLibro.Paginas;
                    }
                    else if (!item.EstaCompletado && item.MiLibroId.HasValue) 
                    {
                        var detallesLibro = _bservice.ObtenerPorId(item.MiLibroId.Value);
                        if (detallesLibro != null) totalPaginas += detallesLibro.Paginas;
                    }
                }
            }
            return totalPaginas;
        }
        //code smell 2
        public void CompletarLibroEnMeta(int idUsuario, int anio, string tituloLibro)
        {
            var metaActual = ObtenerMetaOCrearPorDefecto(idUsuario, anio);
            var libroEnMeta = metaActual.LibrosAsignados.FirstOrDefault(i => i.Titulo == tituloLibro);
        
            if (libroEnMeta != null)
            {
                libroEnMeta.EstaCompletado = true;
                Actualizar(metaActual);
                if (libroEnMeta.MiLibroId.HasValue)
                {
                    var libroReal = _bservice.ObtenerPorId(libroEnMeta.MiLibroId.Value);
                    if(libroReal != null)
                    {
                        libroReal.Estado = "Terminado";
                        _bservice.Actualizar(libroReal);
                    }
                }
                else if(libroEnMeta.RecomendacionId.HasValue)
                {
                    var recomendacion = _rService.ObtenerPorId(libroEnMeta.RecomendacionId.Value);
                    if (recomendacion!= null)
                    {
                        var nuevoLibro = _bservice.PrepararLibroDesdeRecomendacion(recomendacion);
                        nuevoLibro.Estado = "Terminado";
                        _bservice.Agregar(nuevoLibro);
                    }
                }
            }    
        }

    }
}
