using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicLibrary.Infrastructure.Services
{
    public class GroqApiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GroqApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
        }

        public async Task<List<Recommendation>> GenerarRecomendacionesIAAsync(UserProfile perfil)
        {
            if (string.IsNullOrEmpty(_apiKey)) return new List<Recommendation>();

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Eres un bibliotecario experto. Responde ÚNICAMENTE con un arreglo JSON plano, sin bloques de código markdown." },
                    new { role = "user", content = $@"Genera 3 recomendaciones de libros para un lector nivel '{perfil.NivelLector}' con géneros '{perfil.GenerosFavoritos}'.
Formato estricto:
[
  {{
    ""TituloLibro"": ""Título"",
    ""Autor"": ""Autor"",
    ""Genero"": ""Género"",
    ""Razon"": ""Razón corta""
  }}
]" }
                },
                temperature = 0.5
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new List<Recommendation>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                string textoRespuesta = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
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
                // Manejo silencioso para activar el siguiente fallback
            }

            return new List<Recommendation>();
        }
    }
}