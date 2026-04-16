using Microsoft.EntityFrameworkCore;
using Clinica.API.Models.Entities;

namespace Clinica.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // João, aqui você garante que o CPF seja único no MySQL
            modelBuilder.Entity<User>().HasIndex(u => u.Cpf).IsUnique();
        }
    }
}