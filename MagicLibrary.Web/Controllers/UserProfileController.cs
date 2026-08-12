using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        private int ObtenerUserIdEnSesion()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
        }

        [HttpGet]
        public IActionResult Index()
        {
            int userId = ObtenerUserIdEnSesion();
            if (userId == 0) return RedirectToAction("Welcome", "Home");

            // Buscar el perfil de ESTE usuario
            var miPerfil = _userProfileService.ObtenerTodos()
                                             .FirstOrDefault(p => p.UserId == userId);

            if (miPerfil == null)
            {
                return RedirectToAction("Crear");
            }

            return View(miPerfil);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Buscar si el usuario ya tiene un perfil guardado
            var perfilExistente = _userProfileService.ObtenerTodos().FirstOrDefault(p => p.UserId == userId);

            if (perfilExistente != null)
            {
                // Al pasarle el perfil existente a la vista, los inputs se llenarán automáticamente
                return View(perfilExistente);
            }

            // Si es un usuario nuevo sin perfil, se pasa un objeto vacío con fecha actual por defecto
            return View(new UserProfile { UserId = userId, FechaInicio = DateOnly.FromDateTime(DateTime.Now) });
        }
        [HttpPost]
        public IActionResult GuardarPerfil(UserProfile perfil)
        {
            if (!ModelState.IsValid)
            {
                return View("Crear", perfil);
            }

            int userId = ObtenerUserIdEnSesion();
            perfil.UserId = userId; // Asignar el perfil al usuario conectado

            _userProfileService.Agregar(perfil);
            return RedirectToAction("Index", "Home");
        }
    }
}