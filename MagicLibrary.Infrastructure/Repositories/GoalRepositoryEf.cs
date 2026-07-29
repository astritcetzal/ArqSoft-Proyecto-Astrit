using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class GoalRepositoryEf : IGoalRepository
    {
        private readonly MagicLibraryContext _context;

        public GoalRepositoryEf(MagicLibraryContext context)
        {
            _context = context;
        }

        public List<Goal> ObtenerTodos()
        {
            return _context.Goals.ToList();
        }

        public Goal? ObtenerPorId(int id)
        {
            return _context.Goals.FirstOrDefault(b => b.IdMeta == id);
        }

        public void Agregar(Goal meta)
        {
            _context.Goals.Add(meta);
            _context.SaveChanges(); // ¡Así de fácil se guarda en la base de datos!
        }

        public void Actualizar(Goal meta)
        {
            _context.Goals.Update(meta);
            _context.SaveChanges();
        }
    }
}