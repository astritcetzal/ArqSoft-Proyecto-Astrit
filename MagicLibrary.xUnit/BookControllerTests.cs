using System.Security.Claims;
using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MagicLibrary.xUnit
{
    // ====================================================================
    // REPOSITORIOS FAKE (Adaptadores en memoria)
    // ====================================================================
    public class BookRepositoryFake : IBookRepository
    {
        private readonly List<Book> _books;

        public BookRepositoryFake(List<Book> books) => _books = books;

        public List<Book> ObtenerTodos() => _books;

        public Book? ObtenerPorId(int id) => _books.FirstOrDefault(b => b.IdLibro == id);

        public void Agregar(Book libro)
        {
            libro.IdLibro = _books.Count > 0 ? _books.Max(b => b.IdLibro) + 1 : 1;
            _books.Add(libro);
        }

        public void Actualizar(Book libro)
        {
            var index = _books.FindIndex(b => b.IdLibro == libro.IdLibro);
            if (index != -1) _books[index] = libro;
        }
    }

    public class RecommendationRepositoryFake : IRecommendationRepository
    {
        private readonly List<Recommendation> _recommendations;

        public RecommendationRepositoryFake(List<Recommendation> recommendations)
            => _recommendations = recommendations;

        public List<Recommendation> ObtenerTodos() => _recommendations;

        public Recommendation? ObtenerPorId(int id)
            => _recommendations.FirstOrDefault(r => r.Id == id);
    }

    // ====================================================================
    // PRUEBAS DE CONTROLADOR (BookController)
    // ====================================================================

    public class BookControllerTests
    {
        private BookController CrearControllerConDatosDePrueba(out List<Book> librosEsperados)
        {
            // Arrange — Datos de prueba en memoria
            librosEsperados = new List<Book>
            {
                new Book { IdLibro = 1, Titulo = "El Imperio Final", Autor = "Brandon Sanderson", Estado = "Leyendo" },
                new Book { IdLibro = 2, Titulo = "Hábitos Atómicos", Autor = "James Clear", Estado = "Terminado" },
                new Book { IdLibro = 3, Titulo = "Dune", Autor = "Frank Herbert", Estado = "Pendiente" }
            };

            var recomendaciones = new List<Recommendation>
            {
                new Recommendation { Id = 1, TituloLibro = "Cien Años de Soledad", Autor = "Gabriel García Márquez" }
            };

            // Inyección de dependencias usando los Repositorios Fake
            var bookRepoFake = new BookRepositoryFake(librosEsperados);
            var recRepoFake = new RecommendationRepositoryFake(recomendaciones);

            var bookService = new BookService(bookRepoFake);
            var recService = new RecommendationService(recRepoFake);

            var controller = new BookController(bookService, recService);

            // Simular usuario autenticado mediante Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Astrit Cetzal"),
                new Claim(ClaimTypes.Email, "astrit@correo.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        [Fact]
        public void Index_SinFiltro_RegresaTodosLosLibros()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var librosEsperados);

            // Act
            var resultado = controller.Index(null) as ViewResult;
            var modelo = resultado?.Model as List<Book>;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal(librosEsperados.Count, modelo.Count);
        }

        [Fact]
        public void Detalle_ConIdValido_RegresaLibroCorrecto()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out _);

            // Act
            var resultado = controller.Detalle(1) as ViewResult;
            var modelo = resultado?.Model as Book;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal("El Imperio Final", modelo.Titulo);
        }

        [Fact]
        public void Agregar_Post_GuardaLibroYRedirigeAIndex()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var librosIniciales);
            int cantidadInicial = librosIniciales.Count;

            var nuevoLibro = new Book
            {
                Titulo = "Fahrenheit 451",
                Autor = "Ray Bradbury",
                Paginas = 249,
                Estado = "Pendiente"
            };

            // Act
            var resultado = controller.Agregar(nuevoLibro) as RedirectToActionResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Index", resultado.ActionName);
            Assert.Equal(cantidadInicial + 1, librosIniciales.Count);
        }
    }
}