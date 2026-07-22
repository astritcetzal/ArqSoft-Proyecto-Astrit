using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Interfaces
{
    public interface IBookService
    {
        Book PrepararLibroDesdeRecomendacion(Recommendation recommendation);
        List<Book> ObtenerTodos();
        Book? ObtenerPorId(int id);
        void Agregar(Book libro);

        List<Book> ObtenerPorTipoEstado(string tipoEstado);
        List<string> ObtenerTipoEstado();
        void Actualizar(Book libro);
    }
}
