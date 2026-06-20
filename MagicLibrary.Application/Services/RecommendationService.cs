using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;

namespace MagicLibrary.Application.Services
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
    }
}
