using MagicLibrary.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MagicLibrary.Infrastructure.Data
{
    public class MagicLibraryContext : DbContext
    {
        public MagicLibraryContext(DbContextOptions<MagicLibraryContext> options) : base(options)
        {
        }

        // Estas serán tus tablas en la base de datos
        public DbSet<Book> Books { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí puedes definir reglas, por ejemplo, que el IdLibro sea la llave primaria
            modelBuilder.Entity<Book>().HasKey(b => b.IdLibro);
            modelBuilder.Entity<Goal>().HasKey(g => g.IdMeta);
            modelBuilder.Entity<Recommendation>().HasKey(r => r.Id);
            modelBuilder.Entity<UserProfile>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<GoalItem>().HasKey(gi => gi.IdItem);
        }
    }
}