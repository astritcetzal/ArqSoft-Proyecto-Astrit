using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class UserRepositoryEf : IUserRepository
    {
        private readonly MagicLibraryContext _context;

        public UserRepositoryEf(MagicLibraryContext context)
        {
            _context = context;
        }

        public List<User> ObtenerTodos()
        {
            return _context.Users.ToList();
        }

        public User? ObtenerPorId(int id)
        {
            return _context.Users.FirstOrDefault(b => b.Id == id);
        }

        public void Agregar(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); // ¡Así de fácil se guarda en la base de datos!
        }

        public void Actualizar(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}