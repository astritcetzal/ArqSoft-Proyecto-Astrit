using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;

namespace MagicLibrary.Application.Services
{
    public class BookService
    {
        private readonly IBookRepository _repo;
        public BookService(IBookRepository repo)
        {
            _repo = repo;
        }
        public List<Book> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
        public Book? ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }
        public void Agregar(Book libro)
        {
            _repo.Agregar(libro);
        }
        //filtrado

        public List<Book> ObtenerPorTipoEstado(string tipoEstado)
        {
            return _repo.ObtenerTodos().Where(l => l.Estado == tipoEstado).ToList();
        }

        public List<string> ObtenerTipoEstado()
        {
            return _repo.ObtenerTodos().Select(l => l.Estado).Distinct().ToList();
        }
        public void Actualizar(Book libro)
        {
            _repo.Actualizar(libro);
        }

    }
}
