using MagicLibrary.Application.Interfaces;
using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Infrastructure.Data;
using MagicLibrary.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la Base de Datos con EF Core
builder.Services.AddDbContext<MagicLibraryContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MagicLibraryDB;Trusted_Connection=True;"));

// 2. Inyección de IBookRepository usando Decorador (Logging) + EF Core
builder.Services.AddScoped<IGoalObserver, EmailObserver>();
builder.Services.AddScoped<IBookRepository>(sp =>
{
    var context = sp.GetRequiredService<MagicLibraryContext>();
    var repoEf = new BookRepositoryEf(context);
    return new LoggingBookRepository(repoEf);
});

// 3. Repositorios de Infraestructura (EF Core)
builder.Services.AddScoped<IGoalRepository, GoalRepositoryEf>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepositoryEf>();
builder.Services.AddScoped<IUserRepository, UserRepositoryEf>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepositoryEf>();

// 4. Servicios de aplicación
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IGoalService, GoalService>();

// 5. Configurar Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 6. Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 7. Pipeline de la API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();

app.Run();