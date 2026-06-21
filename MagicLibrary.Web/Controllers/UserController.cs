using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace MagicLibrary.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _service;

        // El servicio llega por inyección de dependencias
        public UserController(UserService service)
        {
            _service = service;
        }

        // --- PANTALLA DE REGISTRO ---
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(User user)
        {
            // Ahora sí usamos Agregar
            _service.Agregar(user);

            // Tras registrarse, lo mandamos al catálogo
            return RedirectToAction("Index", "Book");
        }


        // --- PANTALLA DE INICIO DE SESIÓN ---
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contrasena) // <-- Nota el async Task<>
        {
            var usuarios = _service.ObtenerTodos();
            var usuarioValido = usuarios.FirstOrDefault(u => u.Correo == correo && u.Contrasena == contrasena);

            if (usuarioValido != null)
            {
                // 1. Creamos la información del gafete (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioValido.Nombre),
                    new Claim(ClaimTypes.Email, usuarioValido.Correo)
                };

                // 2. Creamos la identidad y le ponemos el sello oficial
                var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Le entregamos el gafete (Cookie) al navegador
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

                return RedirectToAction("Index", "Book");
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
            return View();
        }

        // --- CERRAR SESIÓN ---
        // Este es el método nuevo al que llamará tu botón del menú
        public async Task<IActionResult> CerrarSesion()
        {
            // Le quitamos el gafete (borramos la cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Lo regresamos a la pantalla de inicio principal
            return RedirectToAction("Index", "Home");
        }
    }
}
