using MagicLibraryMVC.Models;

namespace MagicLibraryMVC.Interfaces
{
    public interface IUserRepository
    {
        List<User> ObtenerTodos();
        User? ObtenerPorId(int id);
        void Agregar(User user);
    }
}
