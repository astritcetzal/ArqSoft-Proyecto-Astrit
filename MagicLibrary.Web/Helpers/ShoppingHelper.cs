using System;

namespace MagicLibrary.Web.Helpers
{
    public static class ShoppingHelper
    {
        public static string AmazonUrl(string titulo, string autor)
        {
            string query = Uri.EscapeDataString($"{titulo} {autor} libro");
            return $"https://www.amazon.com.mx/s?k={query}";
        }

        public static string MercadoLibreUrl(string titulo, string autor)
        {
            string query = Uri.EscapeDataString($"{titulo} {autor}");
            return $"https://listado.mercadolibre.com.mx/{query}";
        }
    }
}