using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Web.Controllers;
using MagicLibrary.xUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace MagicLibrary.xUnix
{
    // Fake para Metas
    public class GoalRepositoryFake : IGoalRepository
    {
        private readonly List<Goal> _goals;
        public GoalRepositoryFake(List<Goal> goals) => _goals = goals;

        public List<Goal> ObtenerTodos() => _goals;
        public Goal? ObtenerPorId(int id) => _goals.Find(g => g.IdMeta == id);
        public void Agregar(Goal goal) => _goals.Add(goal);
        public void Actualizar(Goal goal) { }
    }

    public class GoalControllerTests
    {
        [Fact]
        public void Index_CargaMetaActualYCalculaDiasRestantes()
        {
            // Arrange (Preparar)
            var goalsFake = new List<Goal>
            {
                new Goal { IdMeta = 1, IdUsuario = 1, Anio = DateTime.Now.Year, CantidadObjetivo = 5 }
            };

            var goalRepo = new GoalRepositoryFake(goalsFake);
            var bookRepo = new BookRepositoryFake(new List<Book>());
            var recRepo = new RecommendationRepositoryFake(new List<Recommendation>());

            var bService = new BookService(bookRepo);
            var rService = new RecommendationService(recRepo);
            var gService = new GoalService(goalRepo, new List<IGoalObserver>(), bService, rService);

            var controller = new GoalController(gService, rService, bService);

            // Simular autenticación
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Astrit Cetzal") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act (Actuar)
            var resultado = controller.Index() as ViewResult;

            // Assert (Verificar)
            Assert.NotNull(resultado);
            Assert.NotNull(controller.ViewBag.DiasRestantes);
        }
    }
}