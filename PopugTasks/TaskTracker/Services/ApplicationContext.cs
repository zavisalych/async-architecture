using TaskTracker.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Services;

public class ApplicationDbContext : DbContext
{
    public DbSet<Popug> Popugs { get; set; } = null!;

    public DbSet<PopugTask> Tasks { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions options)
                : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Popug>().HasData(new Popug
        {
            Id = 1,
            PublicId = "fb0a961a-c00c-424a-936e-52b9e042b0bc",
            Username = "test@test.com",
            FirstName = "Тест",
            LastName = "Тестов",
            Role = Role.Admin,
        });
    }
}
