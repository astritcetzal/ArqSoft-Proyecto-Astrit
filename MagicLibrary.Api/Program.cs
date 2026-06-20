using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necesario para Swagger
builder.Services.AddSwaggerGen();           // Necesario para Swagger

// Repositorios
builder.Services.AddScoped<IBookRepository, JsonBookRepository>();
builder.Services.AddScoped<IRecommendationRepository, JsonRecommendationRepository>();
// Servicios de aplicación
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<RecommendationService>();

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

// conf swagger 


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
