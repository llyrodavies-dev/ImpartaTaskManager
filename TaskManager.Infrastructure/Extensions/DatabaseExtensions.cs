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
                    job1.UpdateStatus(JobStatus.InProgress, "Ali");
                    job1.AddTask("Install Visual Studio", "Download and install Visual Studio 2022", "system");
                    job1.AddTask("Setup Git", "Configure Git with SSH keys", "system");
                    job1.AddTask("Clone Repository", "Clone the project repository from GitHub", "system");
                    job1.AddTask("Install Dependencies", "Run npm install and restore NuGet packages", "system");

                    await dbContext.Jobs.AddAsync(job1);
                    await dbContext.SaveChangesAsync();

                    var job2 = new Job(domainUser.Id, "Complete API Documentation", "system");
                    job2.AddTask("Document Authentication Endpoints", "Add Swagger documentation for auth endpoints", "system");
                    job2.AddTask("Document Job Endpoints", "Add Swagger documentation for job endpoints", "system");
                    job2.AddTask("Document Task Endpoints", "Add Swagger documentation for task endpoints", "system");
                    job2.AddTask("Create API Examples", "Add example requests and responses", "system");
                    job2.AddTask("Review Documentation", "Peer review of all API documentation", "system");

                    await dbContext.Jobs.AddAsync(job2);
                    await dbContext.SaveChangesAsync();

                    // Update task statuses for job2
                    var job2Tasks = job2.Tasks.ToList();
                    if (job2Tasks.Count >= 5)
                    {
                        job2Tasks[0].UpdateStatus(TaskItemStatus.Completed, "Ali");      // Auth Docs - Completed
                        job2Tasks[1].UpdateStatus(TaskItemStatus.InProgress, "Ali");     // Job Docs - In Progress
                        job2Tasks[2].UpdateStatus(TaskItemStatus.Blocked, "Ali");        // Task Docs - Blocked
                        job2Tasks[3].UpdateStatus(TaskItemStatus.NotStarted, "Ali");     // Examples - Not Started
                        job2Tasks[4].UpdateStatus(TaskItemStatus.NotStarted, "Ali");     // Review - Not Started
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        } 
    }
}
