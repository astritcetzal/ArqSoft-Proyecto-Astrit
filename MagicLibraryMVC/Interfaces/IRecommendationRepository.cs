using MagicLibraryMVC.Models;

namespace MagicLibraryMVC.Interfaces
{
    public interface IRecommendationRepository
    {
        List<Recommendation> ObtenerTodos();
        Recommendation? ObtenerPorId(int id);


        //void Eliminar(int id);

    }
}
