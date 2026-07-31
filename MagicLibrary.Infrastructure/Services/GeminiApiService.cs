using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicLibrary.Infrastructure.Services
{
    public class GeminiApiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        }

        public async Task<List<Recommendation>> GenerarRecomendacionesIAAsync(UserProfile perfil)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("AQUÍ"))
            {
                return new List<Recommendation>();
            }

            try
            {
                string prompt = $@"
Genera exactamente 3 recomendaciones de libros para un lector nivel '{perfil.NivelLector}' con géneros o consulta: '{perfil.GenerosFavoritos}'.
Devuelve ÚNICAMENTE un arreglo JSON plano con este formato exacto:
[
  {{
    ""TituloLibro"": ""Título del libro"",
    ""Autor"": ""Nombre del Autor"",
    ""Genero"": ""Género exacto"",
    ""Razon"": ""Breve razón""
  }}
]";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode) return new List<Recommendation>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                string textoRespuesta = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "[]";

                int inicioJson = textoRespuesta.IndexOf('[');
                int finJson = textoRespuesta.LastIndexOf(']');

                if (inicioJson >= 0 && finJson > inicioJson)
                {
                    textoRespuesta = textoRespuesta.Substring(inicioJson, finJson - inicioJson + 1);
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<Recommendation>>(textoRespuesta, opciones) ?? new List<Recommendation>();
                }
            }
            catch
            {
                // Si falla Gemini, retorna vacío para que salte a Groq
            }

            return new List<Recommendation>();
        }
        public async Task<List<Book>> ExtraerLibrosDeTextoAsync(string textoUsuario)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrWhiteSpace(textoUsuario))
                return new List<Book>();

            try
            {
                string prompt = $@"
        Analiza el siguiente texto y extrae todos los libros mencionados.
        Clasifica cada libro en uno de estos tres estados exactos: 'Terminado', 'Leyendo' o 'Pendiente'.
        Si el usuario no especifica el estado, asígnale 'Pendiente'. Si no se menciona el autor, intenta inferirlo.

        Texto del usuario: ""{textoUsuario}""

        Devuelve ÚNICAMENTE un arreglo JSON con el formato exacto:
        [
          {{
            ""Titulo"": ""Nombre del Libro"",
            ""Autor"": ""Autor"",
            ""Estado"": ""Terminado""
          }}
        ]";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode) return new List<Book>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                string texto = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "[]";

                int inicio = texto.IndexOf('[');
                int fin = texto.LastIndexOf(']');
                if (inicio >= 0 && fin > inicio)
                {
                    texto = texto.Substring(inicio, fin - inicio + 1);
                    return JsonSerializer.Deserialize<List<Book>>(texto, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Book>();
                }
            }
            catch { }

            return new List<Book>();
        }
    }
}