using MagicLibraryMVC.Interfaces;
using MagicLibraryMVC.Models;

namespace MagicLibraryMVC.Services
{
    public class RecommendationService
    {
        private readonly IRecommendationRepository _repo;
        public RecommendationService(IRecommendationRepository repo)
        {
            _repo = repo;
        }
        public List<Recommendation> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
        public Recommendation? ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }
        /// obtener por genero
        /// 
        public List<Recommendation> ObtenerPorGenero(string porGenero)
        {
            return _repo.ObtenerTodos().Where(r => r.Genero == porGenero).ToList();
        }
        public List<string> ObtenerGenero()
        {
            return _repo.ObtenerTodos().Select(r => r.Genero).Distinct().ToList();
        }
    }
}
