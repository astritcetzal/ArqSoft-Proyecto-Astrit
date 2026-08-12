using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using System.Collections.Generic;

namespace MagicLibrary.Application.Services
{
    public class RecommendationFallbackService : IRecommendationFallbackService
    {
        private readonly IGenreNormalizer _normalizer;

        public RecommendationFallbackService(IGenreNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public List<Recommendation> GenerarRespaldo(string tema, string nivel)
        {
            string g = _normalizer.Normalizar(tema);
            return new List<Recommendation>
            {
                new Recommendation
                {
                    TituloLibro = $"Libro Destacado de {g}",
                    Autor = "Autor Recomendado",
                    Genero = g,
                    Razon = $"Sugerencia recomendada para nivel {nivel}.",
                    Paginas = 280
                }
            };
        }
    }
}