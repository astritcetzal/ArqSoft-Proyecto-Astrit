using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Hosting;

using System.Text.Json;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class JsonRecommendationRepository:IRecommendationRepository
    {
        private readonly string _path;
        public JsonRecommendationRepository(IWebHostEnvironment env)
        {
            _path = Path.Combine(env.ContentRootPath, "data", "recomendacion.json");
        }

        public List<Recommendation> ObtenerTodos()
        {
            if (!File.Exists(_path)) return new();


            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Recommendation>>(json) ?? new();
        }
        public Recommendation? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(l => l.Id == id);
        }
    }
}
