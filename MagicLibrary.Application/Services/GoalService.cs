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
        public GoalService(IGoalRepository repo, IEnumerable<IGoalObserver> observers,IBookService bservice, IRecommendationService rService)
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
                    DiasPorSemana = 5,
                    HoraNotificacion = "21:00",
                    LibrosAsignados = new List<GoalItem>()
                };
                _repo.Agregar(meta);
            }
            return meta;
        }
        public int CalcularDiasRestantesAnio(int diasPorSemana = 7)
        {
            DateTime hoy = DateTime.Now;
            DateTime finDeAnio = new DateTime(hoy.Year, 12, 31);
            int diasCalendario = (finDeAnio - hoy).Days;
            if (diasCalendario <= 0) return 1;

            double proporcion = (double)diasPorSemana / 7.0;
            int diasEfectivos = (int)Math.Ceiling(diasCalendario * proporcion);

            return diasEfectivos > 0 ? diasEfectivos : 1;
        }

        public int CalcularTotalPaginasPendientes(Goal metaActual)
        {
            int totalPaginas = 0;
            if (metaActual?.LibrosAsignados != null)
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
        public void CompletarLibroEnMeta(int idUsuario, int anio, string tituloLibro)
        {
            var metaActual = ObtenerMetaOCrearPorDefecto(idUsuario, anio);
            var libroEnMeta = metaActual.LibrosAsignados.FirstOrDefault(i => i.Titulo == tituloLibro);

            if (libroEnMeta != null)
            {
                // 1. Marcamos el item de la meta como completado
                libroEnMeta.EstaCompletado = true;
                Actualizar(metaActual);

                // 2. Si el libro ya existía en "Mis Libros", solo actualizamos su estado
                if (libroEnMeta.MiLibroId.HasValue)
                {
                    var libroReal = _bservice.ObtenerPorId(libroEnMeta.MiLibroId.Value);
                    if (libroReal != null)
                    {
                        libroReal.Estado = "Terminado";
                        _bservice.Actualizar(libroReal);
                    }
                }
                // 3. Si el libro viene de una recomendación de la IA, lo creamos y lo guardamos
                else if (libroEnMeta.RecomendacionId.HasValue)
                {
                    var recomendacion = _rService.ObtenerPorId(libroEnMeta.RecomendacionId.Value);
                    if (recomendacion != null)
                    {
                        var nuevoLibro = _bservice.PrepararLibroDesdeRecomendacion(recomendacion);
                        nuevoLibro.Estado = "Terminado";

                        nuevoLibro.UserId = idUsuario;

                        _bservice.Agregar(nuevoLibro);

                        libroEnMeta.MiLibroId = nuevoLibro.IdLibro;
                        Actualizar(metaActual);
                    }
                }
            }
        }
        public List<Goal> ObtenerTodos() => _repo.ObtenerTodos();
        public Goal? ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public void Agregar(Goal goal) => _repo.Agregar(goal);
        public void Actualizar(Goal goal) => _repo.Actualizar(goal);
        public Goal? ObtenerMetaActual(int idUsuario, int anio) => _repo.ObtenerTodos().FirstOrDefault(g => g.IdUsuario == idUsuario && g.Anio == anio);
        public void ConfirmarLibroAgregado(Goal goal)
        {
            foreach (var observer in _observers)
            {
                observer.OnSavedBook(goal);
            }
        }
    }
}