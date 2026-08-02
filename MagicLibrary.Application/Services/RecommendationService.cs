using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLibrary.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _repo;
        private readonly IAiService _aiService;
        private readonly IGenreNormalizer _normalizer;
        private readonly IRecommendationCacheStore _cache;
        private readonly IRecommendationFallbackService _fallback;

        public RecommendationService(
            IRecommendationRepository repo,
            IAiService aiService,
            IGenreNormalizer normalizer,
            IRecommendationCacheStore cache,
            IRecommendationFallbackService fallback)
        {
            _repo = repo;
            _aiService = aiService;
            _normalizer = normalizer;
            _cache = cache;
            _fallback = fallback;
        }

        public List<Recommendation> ObtenerTodos() => _repo.ObtenerTodos();
        public Recommendation? ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public void Agregar(Recommendation recomendacion) => _repo.Agregar(recomendacion);
        public void Eliminar(int id) { if (id > 0) _repo.Eliminar(id); }

        public List<Recommendation> ObtenerPorGenero(string porGenero)
        {
            string g = _normalizer.Normalizar(porGenero);
            return _repo.ObtenerTodos()
                .Where(r => r.Genero != null && _normalizer.Normalizar(r.Genero).Equals(g, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<string> ObtenerGenerosUsuario(int userId)
        {
            return _cache.ObtenerGeneros(userId)
                ?? _repo.ObtenerTodos()
                    .Select(r => _normalizer.Normalizar(r.Genero))
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .Distinct()
                    .ToList();
        }

        public async Task<List<Recommendation>> ObtenerRecomendacionesUsuarioAsync(int userId, string? generoFiltro, string? promptExtra, UserProfile perfil)
        {
            var generosCache = _cache.ObtenerOAgregarGeneros(userId);
            var librosCache = _cache.ObtenerOAgregarLibros(userId);

            CargarGenerosPerfil(perfil, generosCache);

            if (!string.IsNullOrWhiteSpace(promptExtra))
                await ProcesarPromptAsync(userId, promptExtra, perfil, generosCache, librosCache);

            if (!librosCache.Any())
                await CargarInicialAsync(userId, perfil, generosCache, librosCache);

            return FiltrarSiAplica(userId, generoFiltro, perfil, librosCache);
        }

        public void LimpiarMemoriaUsuario(int userId)
        {
            if (userId <= 0) return;
            _cache.Limpiar(userId);
            var guardados = _repo.ObtenerTodos();
            if (guardados != null)
            {
                foreach (var rec in guardados)
                    _repo.Eliminar(rec.Id);
            }
        }

        private void CargarGenerosPerfil(UserProfile perfil, HashSet<string> generosCache)
        {
            var lista = (perfil.GenerosFavoritos ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(_normalizer.Normalizar)
                .Where(g => !string.IsNullOrWhiteSpace(g));

            foreach (var g in lista)
                generosCache.Add(g);
        }

        private async Task ProcesarPromptAsync(int userId, string prompt, UserProfile perfil, HashSet<string> generosCache, List<Recommendation> librosCache)
        {
            string tema = _normalizer.Normalizar(prompt);
            var recs = await _aiService.GenerarRecomendacionesIAAsync(new UserProfile { UserId = perfil.UserId, NivelLector = perfil.NivelLector, GenerosFavoritos = tema })
                       ?? _fallback.GenerarRespaldo(tema, perfil.NivelLector);

            foreach (var rec in recs)
            {
                rec.Genero = _normalizer.Normalizar(rec.Genero ?? tema);
                generosCache.Add(rec.Genero);
                if (!librosCache.Any(l => l.TituloLibro.Equals(rec.TituloLibro, StringComparison.OrdinalIgnoreCase)))
                {
                    librosCache.Add(rec);
                    _repo.Agregar(rec);
                }
            }
        }

        private async Task CargarInicialAsync(int userId, UserProfile perfil, HashSet<string> generosCache, List<Recommendation> librosCache)
        {
            // 1. Obtener unicamente los generos favoritos del usuario en sesion
            var generosUsuario = (perfil.GenerosFavoritos ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(g => _normalizer.Normalizar(g))
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var bd = _repo.ObtenerTodos() ?? new List<Recommendation>();

            // 2. Filtrar solo libros reales de la BD que coincidan con LOS GÉNEROS FAVORITOS de este usuario
            var librosValidos = bd.Where(r =>
                !string.IsNullOrEmpty(r.TituloLibro) &&
                !r.TituloLibro.StartsWith("Libro Destacado", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(r.Genero) &&
                generosUsuario.Contains(_normalizer.Normalizar(r.Genero))
            ).ToList();

            if (librosValidos.Any())
            {
                foreach (var r in librosValidos)
                {
                    string gNorm = _normalizer.Normalizar(r.Genero);
                    generosCache.Add(gNorm);
                    if (!librosCache.Any(l => l.Id == r.Id))
                        librosCache.Add(r);
                }
            }
            else
            {
                // 3. Si no hay libros en BD para este perfil, llamamos a la IA obligatoriamente
                var recsIA = await _aiService.GenerarRecomendacionesIAAsync(perfil);

                if (recsIA != null && recsIA.Any())
                {
                    foreach (var r in recsIA)
                    {
                        r.Genero = _normalizer.Normalizar(r.Genero);
                        generosCache.Add(r.Genero);
                        librosCache.Add(r);
                        _repo.Agregar(r);
                    }
                }
                else
                {
                    // Contingencia solo si la API de IA falla
                    var generosList = generosUsuario.Any() ? generosUsuario.ToList() : new List<string> { "General" };
                    foreach (var g in generosList)
                    {
                        var respaldos = _fallback.GenerarRespaldo(g, perfil.NivelLector);
                        foreach (var r in respaldos.Take(1))
                        {
                            r.Genero = _normalizer.Normalizar(r.Genero);
                            generosCache.Add(r.Genero);
                            librosCache.Add(r);
                        }
                    }
                }
            }
        }

        private List<Recommendation> FiltrarSiAplica(int userId, string? generoFiltro, UserProfile perfil, List<Recommendation> librosCache)
        {
            if (string.IsNullOrEmpty(generoFiltro) || generoFiltro.Equals("TODOS", StringComparison.OrdinalIgnoreCase))
                return librosCache;

            string gLimpio = _normalizer.Normalizar(generoFiltro);
            return librosCache.Where(r => r.Genero != null && _normalizer.Normalizar(r.Genero).Equals(gLimpio, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}