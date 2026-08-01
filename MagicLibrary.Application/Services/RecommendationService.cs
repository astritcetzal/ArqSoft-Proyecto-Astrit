using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
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
            return _repo.ObtenerTodos().Where(r => r.Genero != null && r.Genero.Equals(g, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<string> ObtenerGenerosUsuario(int userId)
        {
            return _cache.ObtenerGeneros(userId) ?? _repo.ObtenerTodos().Select(r => _normalizer.Normalizar(r.Genero)).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct().ToList();
        }

        public async Task<List<Recommendation>> ObtenerRecomendacionesUsuarioAsync(int userId, string? generoFiltro, string? promptExtra, UserProfile perfil)
        {
            var generosCache = _cache.ObtenerOAgregarGeneros(userId);
            var librosCache = _cache.ObtenerOAgregarLibros(userId);

            CargarGenerosPerfil(perfil, generosCache);

            if (!string.IsNullOrWhiteSpace(promptExtra))
                await ProcesarPromptAsync(userId, promptExtra, perfil, generosCache, librosCache);

            if (!librosCache.Any())
                CargarInicial(userId, perfil, generosCache, librosCache);

            return FiltrarSiAplica(userId, generoFiltro, perfil, librosCache);
        }

        public void LimpiarMemoriaUsuario(int userId)
        {
            if (userId <= 0) return;
            _cache.Limpiar(userId);
            var guardados = _repo.ObtenerTodos();
            if (guardados != null) foreach (var rec in guardados) _repo.Eliminar(rec.Id);
        }

        private void CargarGenerosPerfil(UserProfile perfil, HashSet<string> generosCache)
        {
            var lista = (perfil.GenerosFavoritos ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(_normalizer.Normalizar).Where(g => !string.IsNullOrWhiteSpace(g));
            foreach (var g in lista) generosCache.Add(g);
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

        private void CargarInicial(int userId, UserProfile perfil, HashSet<string> generosCache, List<Recommendation> librosCache)
        {
            var bd = _repo.ObtenerTodos();
            if (bd != null && bd.Any())
            {
                foreach (var r in bd) { r.Genero = _normalizer.Normalizar(r.Genero); generosCache.Add(r.Genero); librosCache.Add(r); }
            }
            else
            {
                var generos = generosCache.ToList();
                var baseRecs = generos.Any() ? generos.SelectMany(g => _fallback.GenerarRespaldo(g, perfil.NivelLector).Take(1)).ToList() : _fallback.GenerarRespaldo("General", perfil.NivelLector);
                foreach (var r in baseRecs.Take(3)) { r.Genero = _normalizer.Normalizar(r.Genero); generosCache.Add(r.Genero); librosCache.Add(r); _repo.Agregar(r); }
            }
        }

        private List<Recommendation> FiltrarSiAplica(int userId, string? generoFiltro, UserProfile perfil, List<Recommendation> librosCache)
        {
            if (string.IsNullOrEmpty(generoFiltro)) return librosCache;
            string gLimpio = _normalizer.Normalizar(generoFiltro);
            var filtrados = librosCache.Where(r => r.Genero != null && r.Genero.Equals(gLimpio, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!filtrados.Any())
            {
                foreach (var rec in _fallback.GenerarRespaldo(gLimpio, perfil.NivelLector).Take(3))
                {
                    rec.Genero = gLimpio;
                    librosCache.Add(rec);
                    _repo.Agregar(rec);
                }
                filtrados = librosCache.Where(r => r.Genero != null && r.Genero.Equals(gLimpio, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return filtrados;
        }
    }
}