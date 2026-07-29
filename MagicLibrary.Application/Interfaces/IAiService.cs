using MagicLibrary.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicLibrary.Application.Interfaces
{
    public interface IAiService
    {
        Task<List<Recommendation>> GenerarRecomendacionesIAAsync(UserProfile perfil);
    }
}