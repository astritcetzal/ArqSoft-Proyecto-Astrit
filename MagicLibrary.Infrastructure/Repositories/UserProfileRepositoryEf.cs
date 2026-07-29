using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class UserProfileRepositoryEf : IUserProfileRepository
    {
        private readonly MagicLibraryContext _context;

        public UserProfileRepositoryEf(MagicLibraryContext context)
        {
            _context = context;
        }

        public List<UserProfile> ObtenerTodos()
        {
            return _context.UserProfiles.ToList();
        }

        public UserProfile? ObtenerPorId(int id)
        {
            return _context.UserProfiles.FirstOrDefault(b => b.Id == id);
        }

        public void Agregar(UserProfile user)
        {
            _context.UserProfiles.Add(user);
            _context.SaveChanges(); // ¡Así de fácil se guarda en la base de datos!
        }

        public void Actualizar(UserProfile user)
        {
            _context.UserProfiles.Update(user);
            _context.SaveChanges();
        }
    }
}