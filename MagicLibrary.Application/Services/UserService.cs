using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;

namespace MagicLibrary.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;

        // El servicio recibe el repositorio por constructor
        // No sabe si es JSON, SQL, memoria, etc.
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        public List<User> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }

        public User? ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public void Agregar(User user)
        {
            // Aquí podrías agregar validaciones de negocio
            // Por ejemplo: if (string.IsNullOrEmpty(user.Nombre)) throw...
            _repo.Agregar(user);
        }
    }
}
