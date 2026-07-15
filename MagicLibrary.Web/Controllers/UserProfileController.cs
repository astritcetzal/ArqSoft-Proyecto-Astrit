using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Mvc;

// para buscar claims
using System.Linq;

namespace MagicLibrary.Web.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly UserProfileService _userProfileService;
      

        public UserProfileController(UserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
           
        }
        [HttpGet]
        public IActionResult Index()

        {
            //pruebas cambiar después
            var perfiles = _userProfileService.ObtenerTodos();
            var miPerfil = perfiles.LastOrDefault();
            if(miPerfil == null)
            {
                return RedirectToAction("Crear");
            }
            return View(miPerfil);

        }


        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        
        // GET: UserProfile
        [HttpPost]
        public IActionResult GuardarPerfil(UserProfile perfil)

        {

            if(!ModelState.IsValid)
            {
                return View("Crear",perfil);
            }
            // buscar identificador
            var userIdClaim = User.Claims.FirstOrDefault(c=>c.Type == "UserId");

            if (userIdClaim !=null)
            {
                //convertir texto a entero, el que user vuelve texto
                int idExtraido = int.Parse(userIdClaim.Value);
                perfil.UserId = idExtraido;
            }

            //si todo es correcto mandar el servicio
            _userProfileService.Agregar(perfil);
            return RedirectToAction("Index", "Home");
        }

    }
}
