using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.Extensions.Configuration;

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
            if (string.IsNullOrEmpty(_apiKey))
                return new List<Recommendation>();

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Eres un bibliotecario experto. Responde ÚNICAMENTE con un arreglo JSON plano, sin bloques de código markdown."
                    },
                    new
                    {
                        role = "user",
                        content = $@"Genera 3 recomendaciones de libros para un lector nivel '{perfil.NivelLector}' con géneros '{perfil.GenerosFavoritos}'.
Formato estricto:
[
  {{
    ""TituloLibro"": ""Título"",
    ""Autor"": ""Autor"",
    ""Genero"": ""Género"",
    ""Razon"": ""Razón corta"",
    ""Paginas"": 250
  }}
]"
                    }
                },
                temperature = 0.5
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<Recommendation>();

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
                // Manejo silencioso para permitir la ejecución del orquestador/fallback
            }

            return new List<Recommendation>();
        }

        public async Task<List<Book>> ExtraerLibrosDeTextoAsync(string textoUsuario)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrWhiteSpace(textoUsuario))
                return new List<Book>();

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Extrae libros de textos en español. Responde ÚNICAMENTE un arreglo JSON plano, sin markdown."
                    },
                    new
                    {
                        role = "user",
                        content = $@"Extrae los libros del texto y clasifícalos en: 'Terminado', 'Leyendo' o 'Pendiente'.
Busca o deduce el número aproximado de PÁGINAS que tiene cada libro. Si no lo encuentras, asigna 250 por defecto.

Texto: ""{textoUsuario}""
Formato estricto:
[
  {{
    ""Titulo"": ""..."",
    ""Autor"": ""..."",
    ""Estado"": ""Pendiente"",
    ""Paginas"": 250
  }}
]"
                    }
                }
            };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<Book>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                string texto = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "[]";

                int inicio = texto.IndexOf('[');
                int fin = texto.LastIndexOf(']');

                if (inicio >= 0 && fin > inicio)
                {
                    texto = texto.Substring(inicio, fin - inicio + 1);
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<Book>>(texto, opciones) ?? new List<Book>();
                }
            }
            catch
            {
                // Manejo silencioso para fallback
            }

            return new List<Book>();
        }
    }
}