using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLibrary.Web.Controllers
{
    [Authorize]
    public class RecommendationController : Controller
    {
        private readonly IAiService _aiService;
        private readonly IUserProfileService _uPserv;

        // MEMORIA DE PERSISTENCIA: Guarda los géneros y libros acumulados durante toda la sesión
        private static readonly ConcurrentDictionary<int, HashSet<string>> _generosPersistentes = new();
        private static readonly ConcurrentDictionary<int, List<Recommendation>> _librosPersistentes = new();

        public RecommendationController(IAiService aiService, IUserProfileService uPserv)
        {
            _aiService = aiService;
            _uPserv = uPserv;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? genero)
        {
            return await ObtenerRecomendaciones(genero, null);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarConIA(string? prompt)
        {
            return await ObtenerRecomendaciones(null, prompt);
        }

        private async Task<IActionResult> ObtenerRecomendaciones(string? generoFiltro, string? promptExtra)
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var perfil = _uPserv.ObtenerTodos().FirstOrDefault(p => p.UserId == userId);
            if (perfil == null)
            {
                return RedirectToAction("Crear", "UserProfile");
            }

            // Inicializar memoria para el usuario si es su primera consulta
            _generosPersistentes.TryAdd(userId, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            _librosPersistentes.TryAdd(userId, new List<Recommendation>());

            // Cargar los géneros iniciales del perfil
            if (!string.IsNullOrEmpty(perfil.GenerosFavoritos))
            {
                foreach (var g in perfil.GenerosFavoritos.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(g))
                        _generosPersistentes[userId].Add(g.Trim());
                }
            }

            // 1. SI EL USUARIO PIDE UN NUEVO TEMA EN EL CHAT (ej: "Terror", "Ciencia Ficción", etc.)
            if (!string.IsNullOrWhiteSpace(promptExtra))
            {
                string nuevoTema = promptExtra.Trim();
                var perfilConsulta = new UserProfile
                {
                    UserId = perfil.UserId,
                    NivelLector = perfil.NivelLector,
                    GenerosFavoritos = nuevoTema
                };

                // Consultar IA (Gemini -> Groq -> Respaldo)
                var nuevasRecomendaciones = await _aiService.GenerarRecomendacionesIAAsync(perfilConsulta)
                                            ?? new List<Recommendation>();

                if (!nuevasRecomendaciones.Any())
                {
                    nuevasRecomendaciones = GenerarRespaldo(nuevoTema, perfil.NivelLector);
                }

                // 💾 GUARDAR EN MEMORIA PERSISTENTE (Para que no se borren)
                foreach (var rec in nuevasRecomendaciones)
                {
                    if (string.IsNullOrWhiteSpace(rec.Genero)) rec.Genero = ExtraerEtiquetaLimpia(nuevoTema);
                    _generosPersistentes[userId].Add(rec.Genero);

                    // Evitar duplicados por título
                    if (!_librosPersistentes[userId].Any(l => l.TituloLibro.Equals(rec.TituloLibro, StringComparison.OrdinalIgnoreCase)))
                    {
                        _librosPersistentes[userId].Add(rec);
                    }
                }
            }

            // 2. SI ES LA PRIMERA VEZ QUE ENTRA Y NO HAY LIBROS GUARDADOS
            if (!_librosPersistentes[userId].Any())
            {
                var recomendacionesBase = await _aiService.GenerarRecomendacionesIAAsync(perfil)
                                          ?? GenerarRespaldo(perfil.GenerosFavoritos, perfil.NivelLector);

                foreach (var rec in recomendacionesBase)
                {
                    if (!string.IsNullOrWhiteSpace(rec.Genero)) _generosPersistentes[userId].Add(rec.Genero);
                    _librosPersistentes[userId].Add(rec);
                }
            }

            // 📌 ENVIAR TODOS LOS BOTONES ACUMULADOS (El botón TERROR ya NUNCA va a desaparecer)
            ViewBag.Generos = _generosPersistentes[userId].ToList();
            ViewBag.GeneroActual = generoFiltro;

            // 3. FILTRAR LOS LIBROS ACUMULADOS SEGÚN EL BOTÓN PRESIONADO
            var librosAEnviar = _librosPersistentes[userId];

            if (!string.IsNullOrEmpty(generoFiltro))
            {
                var filtrados = librosAEnviar
                    .Where(r => r.Genero != null && r.Genero.Contains(generoFiltro, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Si aún no tenemos libros en memoria para ese género en específico, los generamos y los guardamos
                if (!filtrados.Any())
                {
                    var perfilFiltro = new UserProfile { UserId = userId, NivelLector = perfil.NivelLector, GenerosFavoritos = generoFiltro };
                    var masLibros = await _aiService.GenerarRecomendacionesIAAsync(perfilFiltro)
                                    ?? GenerarRespaldo(generoFiltro, perfil.NivelLector);

                    foreach (var rec in masLibros)
                    {
                        if (string.IsNullOrWhiteSpace(rec.Genero)) rec.Genero = generoFiltro;
                        _librosPersistentes[userId].Add(rec);
                    }

                    filtrados = _librosPersistentes[userId]
                        .Where(r => r.Genero != null && r.Genero.Contains(generoFiltro, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                librosAEnviar = filtrados;
            }

            return View("Index", librosAEnviar);
        }

        private string ExtraerEtiquetaLimpia(string texto)
        {
            string t = texto.ToLower();
            if (t.Contains("terror") || t.Contains("miedo") || t.Contains("horror")) return "Terror";
            if (t.Contains("comedia") || t.Contains("humor")) return "Comedia";
            if (t.Contains("fantasia") || t.Contains("fantasía")) return "Fantasía";
            if (t.Contains("romance") || t.Contains("amor")) return "Romance";
            if (t.Contains("ciencia")) return "Ciencia Ficción";
            return texto.Length > 15 ? "General" : texto;
        }

        private List<Recommendation> GenerarRespaldo(string tema, string nivel)
        {
            string g = ExtraerEtiquetaLimpia(tema ?? "General");

            if (g == "Terror")
            {
                return new List<Recommendation>
                {
                    new Recommendation { Id = new Random().Next(100, 999), TituloLibro = "El Exorcista", Autor = "William Peter Blatty", Genero = "Terror", Razon = "Clásico absoluto del terror." },
                    new Recommendation { Id = new Random().Next(100, 999), TituloLibro = "La Llorona y otros relatos", Autor = "Varios", Genero = "Terror", Razon = "Leyendas de suspenso y terror." },
                    new Recommendation { Id = new Random().Next(100, 999), TituloLibro = "Corazones de Piedra", Autor = "Joe Hill", Genero = "Terror", Razon = "Narrativa de terror moderno." }
                };
            }

            return new List<Recommendation>
            {
                new Recommendation { Id = new Random().Next(100, 999), TituloLibro = $"Libro Destacado de {g}", Autor = "Autor Recomendado", Genero = g, Razon = $"Sugerencia para nivel {nivel}." },
                new Recommendation { Id = new Random().Next(100, 999), TituloLibro = $"Grandes Historias ({g})", Autor = "Escritor Relevante", Genero = g, Razon = $"Lectura recomendada en {g}." },
                new Recommendation { Id = new Random().Next(100, 999), TituloLibro = $"Clásico de {g}", Autor = "Especialista del Tema", Genero = g, Razon = "Selección especial para tu perfil." }
            };
        }

        [HttpGet]
        public IActionResult LimpiarMemoria()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (userId > 0)
            {
                _generosPersistentes.TryRemove(userId, out _);
                _librosPersistentes.TryRemove(userId, out _);
            }

            return RedirectToAction("Index");
        }
    }
}