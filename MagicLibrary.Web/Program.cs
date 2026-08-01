using MagicLibrary.Application.Interfaces;
using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Infrastructure.Data;
using MagicLibrary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MagicLibrary.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
// Si no han iniciado sesión no se puede acceder
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Home/Welcome";
    });

// Conexión a la Base de Datos SQL Server (LocalDB)
builder.Services.AddDbContext<MagicLibraryContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MagicLibraryDB;Trusted_Connection=True;"));

var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "data");
Directory.CreateDirectory(dataFolder);

builder.Services.AddControllersWithViews();

// 1. INYECTAR REPOSITORIOS (Infraestructura)
builder.Services.AddScoped<IBookRepository, BookRepositoryEf>();
builder.Services.AddScoped<IGoalRepository, GoalRepositoryEf>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepositoryEf>();
builder.Services.AddScoped<IUserRepository, UserRepositoryEf>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepositoryEf>();
builder.Services.AddScoped<IAiService, AiFallbackOrchestrator>();

// Registrar la llamada HTTP a Gemini y Groq
builder.Services.AddHttpClient<GeminiApiService>();
builder.Services.AddHttpClient<GroqApiService>();

// 2. INYECTAR SERVICIOS (Aplicación)
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Inyectar Servicios de Recomendación y Normalización
builder.Services.AddScoped<IGenreNormalizer, GenreNormalizer>();
builder.Services.AddSingleton<IRecommendationCacheStore, RecommendationCacheStore>();
builder.Services.AddScoped<IRecommendationFallbackService, RecommendationFallbackService>();

// Inyectar Servicio de Email
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificacionBackgroundService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=PrincipalInicio}/{id?}");

app.Run();