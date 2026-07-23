using System.Collections.Generic;
using System.Security.Claims;
using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MagicLibrary.xUnix
{
    // Fake para Perfiles
    public class UserProfileRepositoryFake : IUserProfileRepository
    {
        private readonly List<UserProfile> _perfiles;
        public UserProfileRepositoryFake(List<UserProfile> perfiles) => _perfiles = perfiles;

        public List<UserProfile> ObtenerTodos() => _perfiles;
        public UserProfile? ObtenerPorId(int id) => _perfiles.Find(p => p.Id == id);
        public void Agregar(UserProfile perfil) => _perfiles.Add(perfil);
    }

    public class UserProfileControllerTests
    {
        [Fact]
        public void GuardarPerfil_ConDatosValidos_AgregaPerfilYRedirigeAHome()
        {
            // Arrange
            var perfilesFake = new List<UserProfile>();
            var profileRepo = new UserProfileRepositoryFake(perfilesFake);
            var profileService = new UserProfileService(profileRepo);
            var controller = new UserProfileController(profileService);

            // Simular Claim de UserId = 1
            var claims = new List<Claim> { new Claim("UserId", "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var nuevoPerfil = new UserProfile
            {
                NivelLector = "Frecuente",
                CantidadLibrosHistorico = 15,
                GenerosFavoritos = "Fantasía"
            };

            // Act
            var resultado = controller.GuardarPerfil(nuevoPerfil) as RedirectToActionResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Index", resultado.ActionName);
            Assert.Equal("Home", resultado.ControllerName);
            Assert.Single(perfilesFake); // Verifica que se haya guardado 1 perfil
        }
    }
}