using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Identity;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await dbContext.Database.MigrateAsync();

            // Seed data
            await SeedData(dbContext, userManager);

            return app;
        }

        private static async Task SeedData(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            // Check if user already exists
            const string email = "ali@example.com";
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                // Create Identity user
                var identityUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(identityUser, "Password12345!");

                if (result.Succeeded)
                {
                    // Create Domain user
                    User domainUser = new(identityUser.Id, email, "Ali", "system");
                    await dbContext.DomainUsers.AddAsync(domainUser);
                    await dbContext.SaveChangesAsync();
                }
            }
        } 
    }
}
