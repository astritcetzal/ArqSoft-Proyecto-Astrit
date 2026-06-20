namespace MagicLibrary.Domain.Models
{
    public class BookJson
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int Paginas { get; set; }
        public string FechaInicio { get; set; }
        public string Estado { get; set; } //filtrado Pendiente - Terminado - Leyendo
        public string FechaTerminado { get; set; }
        public int? CalificacionPersonal { get; set; }
        public string? ResenaPersonal { get; set; }

    }
}
