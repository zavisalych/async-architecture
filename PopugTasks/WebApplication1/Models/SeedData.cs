using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthN.Services;
using AuthN.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace AuthN.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider services)
    {
        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var firstUser = new User()
            {
                PublicId = "fb0a961a-c00c-424a-936e-52b9e042b0bc",
                FirstName = "Тест",
                LastName = "Тестов",
                UserName = "test@test.com",
                Email = "test@test.com",
                Role = Role.Admin
            };

            if (!context.Users.Any(u => u.UserName == firstUser.UserName))
            {
                var hasher = manager.PasswordHasher;
                firstUser.PasswordHash = hasher.HashPassword(firstUser, "admin");

                Task<IdentityResult> createTask = manager.CreateAsync(firstUser);
                createTask.Wait();

                if (!createTask.Result.Succeeded)
                {
                    throw new Exception(createTask.Result.ToString());
                }

            }
        }
    }
}