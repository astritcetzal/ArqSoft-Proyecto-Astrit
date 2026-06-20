using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace MagicLibrary.Web.Controllers
{
    public class RecomendacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
