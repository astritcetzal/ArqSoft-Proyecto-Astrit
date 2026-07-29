using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class RecommendationRepositoryEf : IRecommendationRepository
    {
        private readonly MagicLibraryContext _context;

        public RecommendationRepositoryEf(MagicLibraryContext context)
        {
            _context = context;
        }

        public List<Recommendation> ObtenerTodos()
        {
            return _context.Recommendations.ToList();
        }

        public Recommendation? ObtenerPorId(int id)
        {
            return _context.Recommendations.FirstOrDefault(b => b.Id == id);
        }

        public void Agregar(Recommendation recommendation)
        {
            _context.Recommendations.Add(recommendation);
            _context.SaveChanges(); // ¡Así de fácil se guarda en la base de datos!
        }

        public void Actualizar(Recommendation recommendation)
        {
            _context.Recommendations.Update(recommendation);
            _context.SaveChanges();
        }
    }
}