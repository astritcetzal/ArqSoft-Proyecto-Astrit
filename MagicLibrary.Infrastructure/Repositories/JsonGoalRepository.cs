using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class JsonGoalRepository : IGoalRepository
    {
        //ruta del archivo json
        private readonly string _file;
        public JsonGoalRepository(string file)
        {
            _file = file;
            // Si la carpeta no existe, crearla
            var carpeta = Path.GetDirectoryName(_file);
            if (!string.IsNullOrEmpty(carpeta))
                Directory.CreateDirectory(carpeta);
        }
        public List<Goal> ObtenerTodos()
        {
            if (!File.Exists(_file))
                return new List<Goal>();

            var json = File.ReadAllText(_file);
            return JsonSerializer.Deserialize<List<Goal>>(json)
                ?? new List<Goal>();
        }
        public Goal? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(g => g.IdMeta == id);
        }

        public void Agregar(Goal goal)
        {
            var goals = ObtenerTodos();

            //auto incrementar el id
            goal.IdMeta = goals.Count > 0
                ? goals.Max(g => g.IdMeta) + 1
                : 1;

            goals.Add(goal);
            Guardar(goals);
        }
        // Método privado: serializa y escribe el archivo
        private void Guardar(List<Goal> goals)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true   // JSON legible para humanos
            };
            var json = JsonSerializer.Serialize(goals, opciones);
            File.WriteAllText(_file, json);
        }
        public void Actualizar(Goal goal)
        {
            var goals = ObtenerTodos();
            //bucar en que posición de la lista está la meta que queremos actualizat
            var index = goals.FindIndex(g => g.IdMeta == goal.IdMeta);

            if (index != -1)
            {
                goals[index] = goal; // reemplazar la vieja con la nueva ya con la palomita
                Guardar(goals);
            }
        }

    }
}