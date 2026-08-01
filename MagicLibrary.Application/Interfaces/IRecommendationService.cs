using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Interfaces
{
    public interface IRecommendationService
    {
        List<Recommendation> ObtenerTodos();
        Recommendation? ObtenerPorId(int id);
        /// obtener por genero
        List<Recommendation> ObtenerPorGenero(string porGenero);
        List<string> ObtenerGenero();
        void Agregar(Recommendation recomendacion);
    }
}
