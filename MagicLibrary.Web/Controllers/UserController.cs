using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MagicLibrary.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(User user)
        {
            // 1. Verificamos si el correo ya existe en la base de datos
            var usuariosExistentes = _service.ObtenerTodos();
            if (usuariosExistentes.Any(u => u.Correo.Equals(user.Correo, StringComparison.OrdinalIgnoreCase)))
            {
                // Si existe, le mostramos un error en la vista y detenemos el registro
                ModelState.AddModelError("Correo", "Este correo ya está registrado. Intenta con otro o inicia sesión.");
                return View(user);
            }

            // 2. Si el correo es nuevo, lo guardamos normalmente
            _service.Agregar(user);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Nombre),
        new Claim(ClaimTypes.Email, user.Correo),
        new Claim("UserId", user.Id.ToString())
    };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

            return RedirectToAction("Crear", "UserProfile");
        }
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contrasena)
        {
            var usuarios = _service.ObtenerTodos();
            var usuarioValido = usuarios.FirstOrDefault(u => u.Correo == correo && u.Contrasena == contrasena);

            if (usuarioValido != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioValido.Nombre),
                    new Claim(ClaimTypes.Email, usuarioValido.Correo),
                    // guardar el id como texto
                    new Claim("UserId", usuarioValido.Id.ToString())
                };
                // 2. Creamos la identidad y le ponemos el sello oficial
                var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Le entregamos el gafete (Cookie) al navegador
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
            return View();
        }

        // --- CERRAR SESIÓN ---
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("PrincipalInicio", "Home");
        }
    }
}
