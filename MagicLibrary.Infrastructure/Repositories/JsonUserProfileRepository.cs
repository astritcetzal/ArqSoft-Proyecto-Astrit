
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace MagicLibrary.Infrastructure.Repositories
{

    public class JsonUserProfileRepository: IUserProfileRepository
    {
        private readonly string _filePath;
       
        public JsonUserProfileRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "data", "userprofile.json");
        }
        public List<UserProfile> ObtenerTodos()
        {
            if (!File.Exists(_filePath)) return new List<UserProfile>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<UserProfile>>(json) ?? new List<UserProfile>();
        }

        public UserProfile? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(p => p.Id == id);
        }

        public void Agregar(UserProfile perfil)
        {
            var perfiles = ObtenerTodos();
            perfil.Id = perfiles.Count > 0
                      ? perfiles.Max(p => p.Id) + 1
                      : 1;
            perfiles.Add(perfil);
            Guardar(perfil);
            
        }
        private void Guardar(UserProfile perfil)
        {
           
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(perfil, opciones);
            File.WriteAllText(_filePath, json);
        }
    }
}
