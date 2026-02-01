using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
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
            var configuration = services.GetRequiredService<IConfiguration>();

            // Check if we should reset the database
            var resetOnStartup = configuration.GetValue<bool>("DatabaseSettings:ResetOnStartup", false);

            if (resetOnStartup)
            {
                await dbContext.Database.EnsureDeletedAsync();
            }

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

                    // Seed Jobs for the user
                    var job1 = new Job(domainUser.Id, "Setup Development Environment", "system");
                    var job2 = new Job(domainUser.Id, "Complete API Documentation", "system");

                    job1.UpdateStatus(JobStatus.InProgress, "Ali");
                    await dbContext.Jobs.AddAsync(job1);
                    await dbContext.Jobs.AddAsync(job2);
                    await dbContext.SaveChangesAsync();
                }
            }
        } 
    }
}
