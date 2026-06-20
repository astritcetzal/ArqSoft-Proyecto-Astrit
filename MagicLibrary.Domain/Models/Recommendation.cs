namespace MagicLibrary.Domain.Models

{ 
    public class Recommendation 
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public string TituloLibro { get; set; }
        public string Autor {  get; set; }
        public string Razon { get; set; }
        public int paginas { get; set; }
        public string genero { get; set; }

    }
}
