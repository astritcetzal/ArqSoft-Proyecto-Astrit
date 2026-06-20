using MagicLibrary.Domain.Models;

namespace MagicLibrary.Domain.Interfaces
{
    public interface IBookRepository
    {
        List<Book> ObtenerTodos();
        Book? ObtenerPorId(int id);

        void Agregar(Book libro);
        //void Eliminar(int id);
        void Actualizar(Book libro);

    }
}
