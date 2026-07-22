using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
public class BookRepositoryFake : IBookRepository

{
    private readonly List<Book> _book;

    private BookRepositoryFake(List<Book> book) => _book = book;

    public List<Book> ObtenerTodos()=> _book;

    public Book? ObtenerPorId(int id) => _book[id];

    public void Agregar(Book libro)
    {

    }
    
    public void Actualizar(Book libro)
    {

    }
}
public class GoalRepositoryFake : IGoalRepository
{
    public void Actualizar(Goal goal)
    {
        throw new NotImplementedException();
    }

    public void Agregar(Goal goal)
    {
        throw new NotImplementedException();
    }

    public Goal? ObtenerPorId(int id)
    {
        throw new NotImplementedException();
    }

    public List<Goal> ObtenerTodos()
    {
        throw new NotImplementedException();
    }
}
public class RecommendationRepository : IRecommendationRepository
{
    public Recommendation? ObtenerPorId(int id)
    {
        throw new NotImplementedException();
    }

    public List<Recommendation> ObtenerTodos()
    {
        throw new NotImplementedException();
    }
}
public class UserProfile : IUserProfileRepository()
{
    public void Agregar(MagicLibrary.Domain.Models.UserProfile perfil)
    {
        throw new NotImplementedException();
    }

    public MagicLibrary.Domain.Models.UserProfile? ObtenerPorId(int id)
    {
        throw new NotImplementedException();
    }

    public List<MagicLibrary.Domain.Models.UserProfile> ObtenerTodos()
    {
        throw new NotImplementedException();
    }
}