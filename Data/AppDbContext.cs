using Microsoft.EntityFrameworkCore;
using Quiniela.Models;

namespace Quiniela.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "SystemAdmin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            }
            );

            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 2,
                Name = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@quiniela.com",
                    FirstName = "System",
                    LastName = "Admin",
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow
                }
            );

        }

    }
}