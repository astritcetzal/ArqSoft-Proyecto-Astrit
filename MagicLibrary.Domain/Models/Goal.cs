namespace MagicLibrary.Domain.Models
{
    public class Goal
    {
        public int IdMeta { get; set; }
        public int IdUsuario { get; set; }
        public int Anio { get; set; }
        public int CantidadObjetivo { get; set; }

        // aqui se guardan las lineas asignadas en la libreta
        public List<GoalItem> LibrosAsignados { get; set; } = new List<GoalItem>();
        // el modelo de cada linea de libretas


    }
    public class GoalItem
    {
        // titulo para ponerlo en la libreta
        public string Titulo { get; set; }
        // el usuario le ha dado palimita?
        public bool EstaCompletado { get; set; }
        // Los rastreadores solo se llena uno de los 2
        public int? RecomendacionId { get; set; }
        public int? MiLibroId { get; set; }
    }
}
