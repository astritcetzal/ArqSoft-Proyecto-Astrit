using MagicLibrary.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicLibrary.Application.Interfaces
{
    public interface IRecommendationService
    {
        List<Recommendation> ObtenerTodos();
        Recommendation? ObtenerPorId(int id);
        List<Recommendation> ObtenerPorGenero(string porGenero);
        List<string> ObtenerGenerosUsuario(int userId);
        void Agregar(Recommendation recomendacion);
        void Eliminar(int id);
        Task<List<Recommendation>> ObtenerRecomendacionesUsuarioAsync(int userId, string? generoFiltro, string? promptExtra, UserProfile perfil);
        void LimpiarMemoriaUsuario(int userId);
    }
}