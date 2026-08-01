using System.Collections.Generic;
namespace MagicLibrary.Domain.Models
{
    public class Goal
    {
        public int IdMeta { get; set; }
        public int IdUsuario { get; set; }
        public int Anio { get; set; }
        public int CantidadObjetivo { get; set; }
        public int DiasPorSemana { get; set; } = 5; 
        public string HoraNotificacion { get; set; } = "21:00"; 
        public List<GoalItem> LibrosAsignados { get; set; } = new List<GoalItem>();
    } 
    public class GoalItem
    {
        public int IdItem { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool EstaCompletado { get; set; }
        public int? RecomendacionId { get; set; }
        public int? MiLibroId { get; set; }
    }
}