using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class LoggingBookRepository : IBookRepository
    {
        private readonly IBookRepository _inner;

        public List<Book> ObtenerTodos()
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ObtenerTodos - inicio");

            var resultado = _inner.ObtenerTodos();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ObtenerTodos - inicio ObtenerTodos - {resultado.Count} registros");

            return resultado;
        }
        public Book? ObtenerPorId(int id)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ObtenerPorId({id}) — inicio");

            var resultado = _inner.ObtenerPorId(id);

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ObtenerPorId({id}) — {(resultado != null ? "encontrado" : "no encontrado")}");

            return resultado;
        }

        public LoggingBookRepository(IBookRepository book)
        {
            _inner = book;
        }
        public void Actualizar(Book libro)
        {
            throw new NotImplementedException();
        }

        public void Agregar(Book libro)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Agregar  — inicio: {libro.Titulo}");
            //llamar repositorio
            _inner.Agregar(libro);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Agregar - éxito: libro guardado en la base de datos");
        }

        

        
    }
}
