using MagicLibrary.Application.Interfaces;
using System.Globalization;

namespace MagicLibrary.Application.Services
{
    public class GenreNormalizer : IGenreNormalizer
    {
        public string Normalizar(string? rawGenre)
        {
            if (string.IsNullOrWhiteSpace(rawGenre))
                return "General";

            // Si contiene comas, toma la primera etiqueta
            string limpia = rawGenre.Split(',')[0].Trim();

            if (string.IsNullOrWhiteSpace(limpia))
                return "General";

            // Formatea dinámicamente cualquier género a Title Case (ej: "ciencia ficción" -> "Ciencia Ficción")
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(limpia.ToLower());
        }
    }
}