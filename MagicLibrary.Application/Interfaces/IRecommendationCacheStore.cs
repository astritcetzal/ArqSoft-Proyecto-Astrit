using MagicLibrary.Domain.Models;
using System.Collections.Generic;

namespace MagicLibrary.Application.Interfaces
{
    public interface IRecommendationCacheStore
    {
        HashSet<string> ObtenerOAgregarGeneros(int userId);
        List<Recommendation> ObtenerOAgregarLibros(int userId);
        List<string>? ObtenerGeneros(int userId);
        void Limpiar(int userId);
    }
}