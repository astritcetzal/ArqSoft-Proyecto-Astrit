using Microsoft.AspNetCore.Mvc;

namespace MagicLibraryMVC.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
