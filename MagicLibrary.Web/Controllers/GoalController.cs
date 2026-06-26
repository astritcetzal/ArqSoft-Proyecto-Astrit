using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreGeneratedDocument;

namespace MagicLibrary.Web.Controllers
{
    public class GoalController : Controller
    {
        private readonly RecommendationService _Rservice;
        private readonly GoalService _Gservice;

        public GoalController(GoalService Gservice,RecommendationService Rservice)
        {
            _Gservice = Gservice;
            _Rservice = Rservice;
        }


        public IActionResult Index()
        {
            ViewBag.Recommendation = _Rservice.ObtenerTodos();
            return View();
        }

        
        public IActionResult ObtenerPorId(int id)
        {
            var goals = _Gservice.ObtenerTodos().Where(g=> g.IdMeta ==id );
            ViewBag.Recommendation = _Rservice.ObtenerTodos();
            return View(goals);
        }
        // POST
        [HttpPost]
        public IActionResult Agregar(Goal goals)
        {
            ViewBag.Recommendation = _Rservice.ObtenerTodos();
            _Gservice.Agregar(goals);
            return RedirectToAction("Index");
        }

        public IActionResult Actualizar(Goal goals)
        {
            return View(goals);
        }

    }
}

