using MagicLibrary.Domain.Models;
using MagicLibrary.Infrastructure.Repositories;
using MagicLibrary.Application.Services;
using Microsoft.AspNetCore.Identity;
using MagicLibrary.Domain.Interfaces;


var builder = WebApplication.CreateBuilder(args);
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
