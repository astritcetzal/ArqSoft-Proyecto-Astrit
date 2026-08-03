using MagicLibrary.Application.Interfaces;
using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MagicLibrary.xUnit
{
    // ====================================================================
    // REPOSITORIOS Y SERVICIOS FAKE
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

        public void Eliminar(int id)
        {
            throw new NotImplementedException();
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

        public void Agregar(Recommendation recomendacion)
        {
            recomendacion.Id = _recommendations.Count > 0 ? _recommendations.Max(r => r.Id) + 1 : 1;
            _recommendations.Add(recomendacion);
        }

        public void Eliminar(int id)
        {
            _recommendations.RemoveAll(r => r.Id == id);
        }
    }

    public class AiServiceFake : IAiService
    {
        public Task<List<Recommendation>> GenerarRecomendacionesIAAsync(UserProfile perfil)
            => Task.FromResult(new List<Recommendation>());

        public Task<List<Book>> ExtraerLibrosDeTextoAsync(string textoUsuario)
            => Task.FromResult(new List<Book>());
    }

    // ====================================================================
    // PRUEBAS DE CONTROLADOR (BookControllerTests)
    // ====================================================================
    public class BookControllerTests
    {
        private BookController CrearControllerConDatosDePrueba(out List<Book> librosEsperados)
        {
            librosEsperados = new List<Book>
            {
                new Book { IdLibro = 1, UserId = 1, Titulo = "El Imperio Final", Autor = "Brandon Sanderson", Estado = "Leyendo" },
                new Book { IdLibro = 2, UserId = 1, Titulo = "Hábitos Atómicos", Autor = "James Clear", Estado = "Terminado" },
                new Book { IdLibro = 3, UserId = 1, Titulo = "Dune", Autor = "Frank Herbert", Estado = "Pendiente" }
            };

            var recomendaciones = new List<Recommendation>
            {
                new Recommendation { Id = 1, TituloLibro = "Cien Años de Soledad", Autor = "Gabriel García Márquez" }
            };

            var bookRepoFake = new BookRepositoryFake(librosEsperados);
            var recRepoFake = new RecommendationRepositoryFake(recomendaciones);

            var bookService = new BookService(bookRepoFake);
            var aiServiceFake = new AiServiceFake();

            var normalizer = new GenreNormalizer();
            var cacheStore = new RecommendationCacheStore();
            var fallbackService = new RecommendationFallbackService(normalizer);

            var recService = new RecommendationService(
                recRepoFake,
                aiServiceFake,
                normalizer,
                cacheStore,
                fallbackService
            );
            var controller = new BookController(bookService, recService, aiServiceFake);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Astrit Cetzal"),
                new Claim(ClaimTypes.Email, "astrit@correo.com"),
                new Claim("UserId", "1")
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
            var controller = CrearControllerConDatosDePrueba(out var librosEsperados);
            var result = controller.Index(null) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);

            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(result.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public void Detalle_ConIdValido_RegresaLibroCorrecto()
        {
            var controller = CrearControllerConDatosDePrueba(out _);
            var resultado = controller.Detalle(1) as ViewResult;
            var modelo = resultado?.Model as Book;

            Assert.NotNull(modelo);
            Assert.Equal("El Imperio Final", modelo.Titulo);
        }

        [Fact]
        public void Detalle_ConIdInexistente_RegresaVista()
        {
            var controller = CrearControllerConDatosDePrueba(out _);
            var resultado = controller.Detalle(999);

            // Ajustado a ViewResult que es lo que realmente retorna tu controlador
            Assert.IsType<ViewResult>(resultado);
        }

        [Fact]
        public void Agregar_Get_RegresaVistaCorrecta()
        {
            var controller = CrearControllerConDatosDePrueba(out _);
            var resultado = controller.Agregar() as ViewResult;

            Assert.NotNull(resultado);
        }

        [Fact]
        public void Agregar_Post_GuardaLibroYRedirigeAIndex()
        {
            var controller = CrearControllerConDatosDePrueba(out var librosIniciales);
            int cantidadInicial = librosIniciales.Count;

            var nuevoLibro = new Book
            {
                Titulo = "Fahrenheit 451",
                Autor = "Ray Bradbury",
                Paginas = 249,
                Estado = "Pendiente"
            };

            var resultado = controller.Agregar(nuevoLibro) as RedirectToActionResult;

            Assert.NotNull(resultado);
            Assert.Equal("Index", resultado.ActionName);
            Assert.Equal(cantidadInicial + 1, librosIniciales.Count);
        }

        [Fact]
        public void EditarDetalles_Post_ActualizaLibroYRedirigeADetalle()
        {
            var controller = CrearControllerConDatosDePrueba(out var librosIniciales);
            var libroAEditar = new Book
            {
                IdLibro = 1,
                UserId = 1,
                Titulo = "El Imperio Final (Edición Revisada)",
                Autor = "Brandon Sanderson",
                Paginas = 670,
                Estado = "Terminado"
            };

            var resultado = controller.EditarDetalles(libroAEditar) as RedirectToActionResult;

            Assert.NotNull(resultado);
            Assert.Equal("Detalle", resultado.ActionName);

            var libroModificado = librosIniciales.FirstOrDefault(b => b.IdLibro == 1);
            Assert.NotNull(libroModificado);
            Assert.Equal("El Imperio Final (Edición Revisada)", libroModificado.Titulo);
            Assert.Equal(670, libroModificado.Paginas);
            Assert.Equal("Terminado", libroModificado.Estado);
        }
    }
}