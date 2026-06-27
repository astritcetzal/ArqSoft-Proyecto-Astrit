using MagicLibrary.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;


namespace MagicLibrary.Infrastructure.Repositories
{
    public class RepositoryFactory
    {
        public static IBookRepository AgregarLibroRepository(string entorno, IWebHostEnvironment env)
        {
            return entorno switch
            {
                "Production" => new MemoryBookRepository(),
                _ => new JsonBookRepository(env)
            };
        }
    }
}
