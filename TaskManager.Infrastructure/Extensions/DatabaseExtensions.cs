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
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            // Check if we should reset the database
            var resetOnStartup = configuration.GetValue<bool>("DatabaseSettings:ResetOnStartup", false);

            if (resetOnStartup)
            {
                await dbContext.Database.EnsureDeletedAsync();
            }

            await dbContext.Database.MigrateAsync();


            // Seed data
            await SeedRoles(roleManager);
            await SeedData(dbContext, userManager);

            return app;
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
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

                var result = await userManager.CreateAsync(identityUser, "Test123!");

                if (result.Succeeded)
                {
                    // Add user to role
                    await userManager.AddToRoleAsync(identityUser, "User");

                    // Create Domain user
                    User domainUser = new(identityUser.Id, email, "Ali", "system");
                    await dbContext.DomainUsers.AddAsync(domainUser);
                    await dbContext.SaveChangesAsync();

                    // Seed Jobs for the user
                    var job1 = new Job(domainUser.Id, "Setup Development Environment", "system");
                    job1.UpdateStatus(JobStatus.InProgress, "Ali");
                    await dbContext.Jobs.AddAsync(job1);
                    await dbContext.SaveChangesAsync();

                    var job1Tasks = new[]
                   {
                        new TaskItem(job1.Id, "Install Visual Studio", "Download and install Visual Studio 2022", "system"),
                        new TaskItem(job1.Id, "Setup Git", "Configure Git with SSH keys", "system"),
                        new TaskItem(job1.Id, "Clone Repository", "Clone the project repository from GitHub", "system"),
                        new TaskItem(job1.Id, "Install Dependencies", "Run npm install and restore NuGet packages", "system")
                    };
                    await dbContext.Tasks.AddRangeAsync(job1Tasks);
                    await dbContext.SaveChangesAsync();

                    var job2 = new Job(domainUser.Id, "Complete API Documentation", "system");
                    await dbContext.Jobs.AddAsync(job2);
                    await dbContext.SaveChangesAsync();

                    var job2Task1 = new TaskItem(job2.Id, "Document Authentication Endpoints", "Add Swagger documentation for auth endpoints", "system");
                    job2Task1.UpdateStatus(TaskItemStatus.Completed, "Ali");

                    var job2Task2 = new TaskItem(job2.Id, "Document Job Endpoints", "Add Swagger documentation for job endpoints", "system");
                    job2Task2.UpdateStatus(TaskItemStatus.InProgress, "Ali");

                    var job2Task3 = new TaskItem(job2.Id, "Document Task Endpoints", "Add Swagger documentation for task endpoints", "system");
                    job2Task3.UpdateStatus(TaskItemStatus.Blocked, "Ali");

                    var job2Task4 = new TaskItem(job2.Id, "Create API Examples", "Add example requests and responses", "system");

                    var job2Task5 = new TaskItem(job2.Id, "Review Documentation", "Peer review of all API documentation", "system");


                    await dbContext.Tasks.AddRangeAsync(new[] { job2Task1, job2Task2, job2Task3, job2Task4, job2Task5 });
                    await dbContext.SaveChangesAsync();
                }
            }
        } 
    }
}
