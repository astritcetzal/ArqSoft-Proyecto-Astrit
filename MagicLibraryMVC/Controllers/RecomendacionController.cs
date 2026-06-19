using Microsoft.AspNetCore.Mvc;

namespace MagicLibraryMVC.Controllers
{
    public class RecomendacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
