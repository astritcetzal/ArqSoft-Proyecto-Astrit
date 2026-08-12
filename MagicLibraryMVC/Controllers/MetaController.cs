using Microsoft.AspNetCore.Mvc;

namespace MagicLibraryMVC.Controllers
{
    public class MetaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
