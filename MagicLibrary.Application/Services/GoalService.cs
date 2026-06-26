using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;

namespace MagicLibrary.Application.Services
{
    public class GoalService
    {
        private readonly IGoalRepository _repo;
        public GoalService(IGoalRepository repo)
        {
            _repo = repo;
        }
        // logica para asegurar que siempre haya una meta para mostrar
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

        // agregar este método que será  util para el controller
        public Goal? ObtenerMetaActual(int idUsuario, int anio)
        {
            return _repo.ObtenerTodos().FirstOrDefault(g => g.IdUsuario == idUsuario && g.Anio == anio);
        }
    }
}
