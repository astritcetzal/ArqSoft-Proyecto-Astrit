using MagicLibraryMVC.Interfaces;
using MagicLibraryMVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
namespace MagicLibraryMVC.Repositories
{
    public class JsonUserRepository : IUserRepository
    {
        private readonly string _filePath;

        public JsonUserRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "data", "users.json");

            // Si la carpeta no existe, crearla
            var carpeta2 = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(carpeta2))
                Directory.CreateDirectory(carpeta2);
        }

        public List<User> ObtenerTodos()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        // Agrega un usuario y guarda la lista completa en el JSON
        public void Agregar(User user)
        {
            var users = ObtenerTodos();

            // Auto-incrementar el Id
            user.Id = users.Count > 0
                      ? users.Max(u => u.Id) + 1
                      : 1;

            users.Add(user);
            Guardar(users);
        }


        public User? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(u => u.Id == id);
        }

        // Método privado: serializa y escribe el archivo
        private void Guardar(List<User> users)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true   // JSON legible para humanos
            };
            var json = JsonSerializer.Serialize(users, opciones);
            File.WriteAllText(_filePath, json);
        }
    }
}
