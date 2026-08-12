using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class BookRepositoryEf : IBookRepository
    {
        private readonly MagicLibraryContext _context;

        public BookRepositoryEf(MagicLibraryContext context)
        {
            _context = context;
        }

        public List<Book> ObtenerTodos()
        {
            return _context.Books.ToList();
        }

        public Book? ObtenerPorId(int id)
        {
            return _context.Books.FirstOrDefault(b => b.IdLibro == id);
        }

        public void Agregar(Book libro)
        {
            _context.Books.Add(libro);
            _context.SaveChanges(); // ¡Así de fácil se guarda en la base de datos!
        }

        public void Actualizar(Book libro)
        {
            _context.Books.Update(libro);
            _context.SaveChanges();
        }
        public void Eliminar(int id)
        {
            var libro = _context.Books.Find(id);
            if (libro != null)
            {
                _context.Books.Remove(libro);
                _context.SaveChanges();
            }
        }   
    }
}