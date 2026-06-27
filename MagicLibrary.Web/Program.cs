using MagicLibrary.Application.Services;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);
// Si no han iniciado sesion no se puede acceder
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Home/Welcome"; 
    });
//----
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "data");
Directory.CreateDirectory(dataFolder);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookRepository, JsonBookRepository>();
builder.Services.AddScoped<IRecommendationRepository, JsonRecommendationRepository>();
builder.Services.AddScoped<IUserRepository, JsonUserRepository>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IGoalRepository, JsonGoalRepository>();
builder.Services.AddScoped<GoalService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // junto a cookies ya sabes :/
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=PrincipalInicio}/{id?}")
    ;


app.Run();
