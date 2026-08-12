using MagicLibrary.Domain.Models;

namespace MagicLibrary.Domain.Interfaces
{
    public interface IGoalRepository
    {
        List<Goal> ObtenerTodos();
        Goal? ObtenerPorId(int id);
        void Agregar(Goal goal);
        void Actualizar(Goal goal);
    }
}
