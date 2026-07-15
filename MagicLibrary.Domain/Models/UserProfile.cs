using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagicLibrary.Domain.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "El nivel de lector es obligatorio.")]
        public string NivelLector { get; set; }
        [Required(ErrorMessage = "Por favor, dinos cuantos libros has leido.")]
        [Range(0,10000, ErrorMessage = "La cantidad debe ser un número válida." )]
        public int CantidadLibrosHistorico { get; set; }
        public DateOnly FechaInicio { get; set; }
        public string GenerosFavoritos { get; set; }

    }
}
