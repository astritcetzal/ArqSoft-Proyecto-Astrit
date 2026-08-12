using MagicLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class RecommendationController : Controller
    {
        private readonly IUserProfileService _uPserv;
        private readonly IRecommendationService _recServ;

        public RecommendationController(
            IUserProfileService uPserv,
            IRecommendationService recServ)
        {
            _uPserv = uPserv;
            _recServ = recServ;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? genero)
        {
            return await ProcesarRecomendaciones(genero, null);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarConIA(string? prompt)
        {
            return await ProcesarRecomendaciones(null, prompt);
        }

        private async Task<IActionResult> ProcesarRecomendaciones(string? generoFiltro, string? promptExtra)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var perfil = _uPserv.ObtenerTodos().FirstOrDefault(p => p.UserId == userId);
            if (perfil == null)
            {
                return RedirectToAction("Crear", "UserProfile");
            }

            var libros = await _recServ.ObtenerRecomendacionesUsuarioAsync(userId, generoFiltro, promptExtra, perfil);

            ViewBag.Generos = _recServ.ObtenerGenerosUsuario(userId);
            ViewBag.GeneroActual = generoFiltro;

            return View("Index", libros);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult Eliminar(int id, string? titulo)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var guardados = _recServ.ObtenerTodos();
            var aEliminar = guardados.FirstOrDefault(r => (id > 0 && r.Id == id)
                                                       || (!string.IsNullOrEmpty(titulo) && r.TituloLibro.Equals(titulo, System.StringComparison.OrdinalIgnoreCase)));

            if (aEliminar != null)
            {
                _recServ.Eliminar(aEliminar.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult LimpiarMemoria()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            _recServ.LimpiarMemoriaUsuario(userId);

            return RedirectToAction("Index");
        }
    }
}