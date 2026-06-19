using MagicLibraryMVC.Interfaces;
using MagicLibraryMVC.Models;
using System.Text.Json;

namespace MagicLibraryMVC.Repositories
{
    public class JsonBookRepository: IBookRepository
    {
        private readonly string _path;
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        public JsonBookRepository(IWebHostEnvironment env)
        {
            _path = Path.Combine(env.ContentRootPath, "data", "libro.json");
        }
        public List<Book> ObtenerTodos()
        {
            if (!File.Exists(_path)) return new();
            var json = File.ReadAllText(_path);
            var libroJson = JsonSerializer.Deserialize < List<BookJson >> (json , _options) ?? new();
            return libroJson.Select(l => new Book
            {
                IdLibro = l.IdLibro,
                Titulo = l.Titulo,
                Autor = l.Autor,
                Paginas = l.Paginas,
                FechaInicio = string.IsNullOrEmpty(l.FechaInicio) ? null : DateOnly.Parse(l.FechaInicio),
                Estado = l.Estado,
                FechaTerminado = string.IsNullOrEmpty(l.FechaTerminado) ? null : DateOnly.Parse(l.FechaTerminado),

                CalificacionPersonal = l.CalificacionPersonal,
                ResenaPersonal = l.ResenaPersonal
            }).ToList();
        }
        public Book? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(l => l.IdLibro == id);
        }
        public void Agregar(Book libro)
        {
            var libros = ObtenerTodos();
            //INCREMENTAR EL ID 
            libro.IdLibro = libros.Count > 0
                ? libros.Max(i => i.IdLibro) + 1
                : 1;
            libros.Add(libro);
            Guardar(libros);
        }
        public void Guardar(List<Book> libros)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true  //json legible para personas
            };
            var json = JsonSerializer.Serialize(libros, opciones);
            File.WriteAllText(_path, json);
        }

        public void Actualizar(Book libroAct)
        {
            var libros = ObtenerTodos();
            //buscar el indice de la reseña original en la lista 
            var index = libros.FindIndex(r => r.IdLibro == libroAct.IdLibro);
            if (index != -1)
            {
                libros[index] = libroAct;
                Guardar(libros);
            }
        }
    }
}
 