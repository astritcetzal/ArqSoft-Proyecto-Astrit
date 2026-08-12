namespace MagicLibraryMVC.Models
{
    public class Book
    {
        public int IdLibro { get; set;  }
        public string Titulo    { get; set; }
        public string Autor {  get; set; }
        public int Paginas { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public string Estado { get; set; } //filtrado Pendiente - Terminado - Leyendo
        public DateOnly? FechaTerminado{  get; set; }

        public int? CalificacionPersonal { get; set; }
        public string? ResenaPersonal { get; set; }

    }
}
