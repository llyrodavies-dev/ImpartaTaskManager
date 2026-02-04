using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Interfaces;
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
            var jobStatusService = services.GetRequiredService<IJobStatusService>();

            // Check if we should reset the database
            var resetOnStartup = configuration.GetValue<bool>("DatabaseSettings:ResetOnStartup", false);

            if (resetOnStartup)
            {
                await dbContext.Database.EnsureDeletedAsync();
            }

            await dbContext.Database.MigrateAsync();


            // Seed data
            await SeedRoles(roleManager);
            await SeedData(dbContext, userManager, jobStatusService);

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

        private static async Task SeedData(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IJobStatusService jobStatusService)
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

                    // Job 1: Client Onboarding Process (Created 15 days ago)
                    var job1Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO Jobs (Id, UserId, Title, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        job1Id, domainUser.Id, "Q1 2026 Client Onboarding Process", (int)JobStatus.NotStarted, DateTime.UtcNow.AddDays(-15), "system");

                    var job1Task1Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job1Task1Id, job1Id, "Prepare Client Welcome Pack", 
                        "Organise and compile welcome documentation, company policies, and initial project brief for new client",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-15), "system", DateTime.UtcNow.AddDays(-14), "Ali");

                    var job1Task2Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job1Task2Id, job1Id, "Schedule Initial Consultation", 
                        "Arrange introductory meeting with stakeholders to discuss project requirements and timelines",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-14), "system", DateTime.UtcNow.AddDays(-13), "Ali");

                    var job1Task3Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job1Task3Id, job1Id, "Conduct Requirements Analysis", 
                        "Analyse client requirements and document functional and non-functional specifications",
                        (int)TaskItemStatus.InProgress, DateTime.UtcNow.AddDays(-10), "system", DateTime.UtcNow.AddDays(-9), "Ali");

                    var job1Task4Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job1Task4Id, job1Id, "Submit Proposal Document", 
                        "Finalise and submit detailed project proposal including costings and resource allocation",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-8), "system");

                    // Update Job 1 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job1Id, "Ali");

                    // Job 2: Annual Health & Safety Compliance Review (Created 30 days ago)
                    var job2Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO Jobs (Id, UserId, Title, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        job2Id, domainUser.Id, "Annual Health & Safety Compliance Review", (int)JobStatus.NotStarted, DateTime.UtcNow.AddDays(-30), "system");

                    var job2Task1Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job2Task1Id, job2Id, "Review Risk Assessment Documentation", 
                        "Examine all current risk assessments and ensure compliance with HSE regulations",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-30), "system", DateTime.UtcNow.AddDays(-28), "Ali");

                    var job2Task2Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job2Task2Id, job2Id, "Organise Fire Safety Training", 
                        "Coordinate fire warden training sessions for all floor managers across UK offices",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-25), "system", DateTime.UtcNow.AddDays(-23), "Ali");

                    var job2Task3Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job2Task3Id, job2Id, "Update Emergency Procedures", 
                        "Revise and distribute updated emergency evacuation procedures to all staff members",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-20), "system", DateTime.UtcNow.AddDays(-18), "Ali");

                    var job2Task4Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job2Task4Id, job2Id, "Commission External Safety Audit", 
                        "Engage approved HSE consultant to conduct comprehensive workplace safety inspection",
                        (int)TaskItemStatus.InProgress, DateTime.UtcNow.AddDays(-12), "system", DateTime.UtcNow.AddDays(-11), "Ali");

                    var job2Task5Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job2Task5Id, job2Id, "Prepare Compliance Report", 
                        "Compile findings and submit annual health and safety compliance report to senior management",
                        (int)TaskItemStatus.Blocked, DateTime.UtcNow.AddDays(-5), "system", DateTime.UtcNow.AddDays(-4), "Ali");

                    // Update Job 2 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job2Id, "Ali");

                    // Job 3: Marketing Campaign Launch (Created 3 days ago)
                    var job3Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO Jobs (Id, UserId, Title, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        job3Id, domainUser.Id, "Spring Marketing Campaign Launch", (int)JobStatus.NotStarted, DateTime.UtcNow.AddDays(-3), "system");

                    var job3Task1Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job3Task1Id, job3Id, "Develop Campaign Strategy", 
                        "Create comprehensive marketing strategy including target demographics, channels, and key messaging",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-3), "system");

                    var job3Task2Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job3Task2Id, job3Id, "Design Creative Assets", 
                        "Commission design team to produce promotional materials, social media graphics, and email templates",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-3), "system");

                    var job3Task3Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job3Task3Id, job3Id, "Coordinate PR Activities", 
                        "Liaise with public relations agency to organise press releases and media engagement opportunities",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-2), "system");

                    var job3Task4Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job3Task4Id, job3Id, "Implement Social Media Schedule", 
                        "Deploy scheduled posts across LinkedIn, Twitter, and Instagram platforms throughout campaign period",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-2), "system");

                    var job3Task5Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        job3Task5Id, job3Id, "Monitor Campaign Analytics", 
                        "Track engagement metrics, conversion rates, and return on investment using analytics platforms",
                        (int)TaskItemStatus.NotStarted, DateTime.UtcNow.AddDays(-1), "system");

                    // Update Job 3 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job3Id, "Ali");

                    // Job 4: Office Relocation Project (Created 60 days ago, completed)
                    var job4Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO Jobs (Id, UserId, Title, Status, CreatedAtUtc, CreatedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        job4Id, domainUser.Id, "Manchester Office Relocation Project", (int)JobStatus.NotStarted, DateTime.UtcNow.AddDays(-60), "system");

                    var job4Task1Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task1Id, job4Id, "Source Suitable Premises", 
                        "Identify and evaluate potential office spaces within Manchester city centre meeting capacity requirements",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-60), "system", DateTime.UtcNow.AddDays(-58), "Ali");

                    var job4Task2Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task2Id, job4Id, "Negotiate Lease Agreement", 
                        "Finalise terms and conditions with landlord including rent, service charges, and break clauses",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-55), "system", DateTime.UtcNow.AddDays(-53), "Ali");

                    var job4Task3Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task3Id, job4Id, "Arrange Office Fit-Out", 
                        "Coordinate with contractors to install workstations, meeting rooms, and kitchen facilities",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-50), "system", DateTime.UtcNow.AddDays(-48), "Ali");

                    var job4Task4Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task4Id, job4Id, "Organise IT Infrastructure", 
                        "Ensure installation of network cabling, telecommunications, and server equipment prior to move date",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-45), "system", DateTime.UtcNow.AddDays(-43), "Ali");

                    var job4Task5Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task5Id, job4Id, "Plan Staff Relocation", 
                        "Communicate moving schedule to employees and arrange transport for equipment and personal belongings",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-40), "system", DateTime.UtcNow.AddDays(-38), "Ali");

                    var job4Task6Id = Guid.NewGuid();
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"INSERT INTO TaskItems (Id, JobId, Title, Description, Status, CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy) 
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        job4Task6Id, job4Id, "Update Business Address", 
                        "Notify Companies House, HMRC, clients, and suppliers of new registered office address",
                        (int)TaskItemStatus.Completed, DateTime.UtcNow.AddDays(-35), "system", DateTime.UtcNow.AddDays(-33), "Ali");

                    // Update Job 4 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job4Id, "Ali");
                }
            }
        } 
    }
}
