using Xunit;

namespace MagicLibrary.Tests
{
    public class PruebasIniciales
    {
        [Fact]
        public void PruebaIntencionalQueFalla()
        {
            // Esta prueba va a fallar para verificar que GitHub Actions detecte el error en rojo
            Assert.True(false, "Fallo intencional para verificar el check rojo de CI.");
        }
    }
}