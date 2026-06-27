using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGoalObserver, EmailObserver>();
builder.Services.AddScoped<IBookRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var repo = RepositoryFactory.AgregarLibroRepository(builder.Environment.EnvironmentName, env);
    return new LoggingBookRepository(repo);
});
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necesario para Swagger
builder.Services.AddSwaggerGen();           // Necesario para Swagger

// Repositorios

builder.Services.AddScoped<IRecommendationRepository, JsonRecommendationRepository>();
builder.Services.AddScoped<IGoalRepository, JsonGoalRepository>();
// Servicios de aplicación
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<GoalService>();

//nuevooo
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", app =>
    {
        app.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

//nuevooo
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", app =>
    {
        app.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});
var app = builder.Build();

// 4. Configurar el pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // ¡Esto es vital para tu API!

app.Run();
