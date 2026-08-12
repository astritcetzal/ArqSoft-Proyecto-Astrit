using MagicLibrary.Domain.Models;

namespace MagicLibrary.Domain.Interfaces
{
    public interface IUserRepository
    {
        List<User> ObtenerTodos();
        User? ObtenerPorId(int id);
        void Agregar(User user);
    }
}
