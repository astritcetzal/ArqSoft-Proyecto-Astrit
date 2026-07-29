using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicLibrary.Infrastructure.Services
{
    public class AiFallbackOrchestrator : IAiService
    {
        private readonly GeminiApiService _geminiService;
        private readonly GroqApiService _groqService;

        public AiFallbackOrchestrator(GeminiApiService geminiService, GroqApiService groqService)
        {
            _geminiService = geminiService;
            _groqService = groqService;
        }

        public async Task<List<Recommendation>> GenerarRecomendacionesIAAsync(UserProfile perfil)
        {
            // 1. Intentar con Gemini
            var resultados = await _geminiService.GenerarRecomendacionesIAAsync(perfil);
            if (resultados != null && resultados.Count > 0) return resultados;

            // 2. Si Gemini falla, Intentar con Groq (Llama 3.3)
            resultados = await _groqService.GenerarRecomendacionesIAAsync(perfil);
            if (resultados != null && resultados.Count > 0) return resultados;

            // 3. Respaldo Local Inteligente (si ambas APIs fallan)
            return GenerarRespaldoLocal(perfil);
        }

        private List<Recommendation> GenerarRespaldoLocal(UserProfile perfil)
        {
            string tema = (perfil.GenerosFavoritos ?? "").ToLower();

            if (tema.Contains("terror") || tema.Contains("miedo") || tema.Contains("horror"))
            {
                return new List<Recommendation>
                {
                    new Recommendation { Id = 1, TituloLibro = "Drácula", Autor = "Bram Stoker", Genero = "Terror", Razon = $"Clásico de terror para nivel {perfil.NivelLector}." },
                    new Recommendation { Id = 2, TituloLibro = "El Resplandor", Autor = "Stephen King", Genero = "Terror", Razon = "Suspenso y terror psicológico." },
                    new Recommendation { Id = 3, TituloLibro = "Frankenstein", Autor = "Mary Shelley", Genero = "Terror", Razon = "Clásico gótico imprescindible." }
                };
            }

            if (tema.Contains("comedia") || tema.Contains("humor") || tema.Contains("risas"))
            {
                return new List<Recommendation>
                {
                    new Recommendation { Id = 1, TituloLibro = "Guía del Autoestopista Galáctico", Autor = "Douglas Adams", Genero = "Comedia", Razon = "Humor absurdo e inteligente." },
                    new Recommendation { Id = 2, TituloLibro = "Sin Noticias de Gurb", Autor = "Eduardo Mendoza", Genero = "Comedia", Razon = "Sátira muy divertida y ligera." },
                    new Recommendation { Id = 3, TituloLibro = "Maldito Karma", Autor = "David Safier", Genero = "Comedia", Razon = "Comedia fresca e hilarante." }
                };
            }

            if (tema.Contains("fantasia") || tema.Contains("fantasía"))
            {
                return new List<Recommendation>
                {
                    new Recommendation { Id = 1, TituloLibro = "El Nombre del Viento", Autor = "Patrick Rothfuss", Genero = "Fantasía", Razon = "Fantasía épica fascinante." },
                    new Recommendation { Id = 2, TituloLibro = "El Hobbit", Autor = "J.R.R. Tolkien", Genero = "Fantasía", Razon = "Aventura clásica de fantasía." },
                    new Recommendation { Id = 3, TituloLibro = "Nacidos de la Bruma", Autor = "Brandon Sanderson", Genero = "Fantasía", Razon = "Sistema de magia único." }
                };
            }

            if (tema.Contains("romance") || tema.Contains("amor"))
            {
                return new List<Recommendation>
                {
                    new Recommendation { Id = 1, TituloLibro = "Orgullo y Prejuicio", Autor = "Jane Austen", Genero = "Romance", Razon = "Un clásico inolvidable del romance." },
                    new Recommendation { Id = 2, TituloLibro = "Cumbres Borrascosas", Autor = "Emily Brontë", Genero = "Romance", Razon = "Intensa historia de amor." },
                    new Recommendation { Id = 3, TituloLibro = "Yo Antes de Ti", Autor = "Jojo Moyes", Genero = "Romance", Razon = "Emotiva historia contemporánea." }
                };
            }

            // Para cualquier otro género dinámico
            string generoGenerico = string.IsNullOrWhiteSpace(perfil.GenerosFavoritos) ? "General" : perfil.GenerosFavoritos.Trim();
            if (generoGenerico.Length > 20) generoGenerico = "Recomendado";

            return new List<Recommendation>
            {
                new Recommendation { Id = 1, TituloLibro = $"Grandes Obras de {generoGenerico}", Autor = "Autor Destacado", Genero = generoGenerico, Razon = $"Sugerencia para nivel {perfil.NivelLector}." },
                new Recommendation { Id = 2, TituloLibro = $"Antología de {generoGenerico}", Autor = "Escritor Relevante", Genero = generoGenerico, Razon = $"Lectura recomendada en {generoGenerico}." },
                new Recommendation { Id = 3, TituloLibro = $"Lectura Clave de {generoGenerico}", Autor = "Especialista del Tema", Genero = generoGenerico, Razon = $"Adaptado a tus preferencias." }
            };
        }
    }
}