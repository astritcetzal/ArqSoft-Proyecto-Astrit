using MagicLibrary.Domain.Models;
using System.Collections.Generic;

namespace MagicLibrary.Application.Interfaces
{
    public interface IRecommendationFallbackService
    {
        List<Recommendation> GenerarRespaldo(string tema, string nivel);
    }
}