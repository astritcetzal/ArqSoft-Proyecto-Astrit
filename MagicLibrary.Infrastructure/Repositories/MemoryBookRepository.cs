using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class MemoryBookRepository : IBookRepository
    {
        public void Actualizar(Book libro)
        {
            throw new NotImplementedException();
        }

        public void Agregar(Book libro)
        {
            throw new NotImplementedException();
        }

        public Book? ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<Book> ObtenerTodos()
        {
            throw new NotImplementedException();
        }
    }
}
