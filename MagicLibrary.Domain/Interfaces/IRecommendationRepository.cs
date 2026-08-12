using MagicLibrary.Domain.Models;

namespace MagicLibrary.Domain.Interfaces
{
    public interface IRecommendationRepository
    {
        List<Recommendation> ObtenerTodos();
        Recommendation? ObtenerPorId(int id);

        void Agregar(Recommendation recomendacion);
        void Eliminar(int id);

    }
}
