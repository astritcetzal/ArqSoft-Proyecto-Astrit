using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; // 👈 OBLIGATORIO PARA EL .Include()
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
            // 🔑 EL .Include TRAE LOS LIBROS ASIGNADOS DE LA BASE DE DATOS
            return _context.Goals
                           .Include(g => g.LibrosAsignados)
                           .ToList();
        }

        public Goal? ObtenerPorId(int id)
        {
            return _context.Goals
                           .Include(g => g.LibrosAsignados)
                           .FirstOrDefault(b => b.IdMeta == id);
        }

        public void Agregar(Goal meta)
        {
            _context.Goals.Add(meta);
            _context.SaveChanges();
        }

        public void Actualizar(Goal meta)
        {
            _context.Goals.Update(meta);
            _context.SaveChanges(); // Persiste los GoalItem agregados
        }
    }
}