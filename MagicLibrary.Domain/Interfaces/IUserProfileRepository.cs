using MagicLibrary.Domain.Models;


namespace MagicLibrary.Domain.Interfaces
{
    public interface IUserProfileRepository
    {
        List<UserProfile> ObtenerTodos();
        UserProfile? ObtenerPorId(int id);
        void Agregar(UserProfile perfil);
    }
}
