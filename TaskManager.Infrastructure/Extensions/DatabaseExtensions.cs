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

                    // Job 1: Client Onboarding Process
                    var job1 = new Job(domainUser.Id, "Q1 2026 Client Onboarding Process", "system");
                    await dbContext.Jobs.AddAsync(job1);
                    await dbContext.SaveChangesAsync();

                    var job1Tasks = new[]
                    {
                        new TaskItem(job1.Id, "Prepare Client Welcome Pack", "Organise and compile welcome documentation, company policies, and initial project brief for new client", "system"),
                        new TaskItem(job1.Id, "Schedule Initial Consultation", "Arrange introductory meeting with stakeholders to discuss project requirements and timelines", "system"),
                        new TaskItem(job1.Id, "Conduct Requirements Analysis", "Analyse client requirements and document functional and non-functional specifications", "system"),
                        new TaskItem(job1.Id, "Submit Proposal Document", "Finalise and submit detailed project proposal including costings and resource allocation", "system")
                    };
                    job1Tasks[0].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job1Tasks[1].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job1Tasks[2].UpdateStatus(TaskItemStatus.InProgress, "Ali");
                    // job1Tasks[3] remains NotStarted
                    await dbContext.Tasks.AddRangeAsync(job1Tasks);
                    await dbContext.SaveChangesAsync();

                    // Update Job 1 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job1.Id, "Ali");

                    // Job 2: Annual Health & Safety Compliance Review
                    var job2 = new Job(domainUser.Id, "Annual Health & Safety Compliance Review", "system");
                    await dbContext.Jobs.AddAsync(job2);
                    await dbContext.SaveChangesAsync();

                    var job2Tasks = new[]
                    {
                        new TaskItem(job2.Id, "Review Risk Assessment Documentation", "Examine all current risk assessments and ensure compliance with HSE regulations", "system"),
                        new TaskItem(job2.Id, "Organise Fire Safety Training", "Coordinate fire warden training sessions for all floor managers across UK offices", "system"),
                        new TaskItem(job2.Id, "Update Emergency Procedures", "Revise and distribute updated emergency evacuation procedures to all staff members", "system"),
                        new TaskItem(job2.Id, "Commission External Safety Audit", "Engage approved HSE consultant to conduct comprehensive workplace safety inspection", "system"),
                        new TaskItem(job2.Id, "Prepare Compliance Report", "Compile findings and submit annual health and safety compliance report to senior management", "system")
                    };
                    job2Tasks[0].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job2Tasks[1].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job2Tasks[2].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job2Tasks[3].UpdateStatus(TaskItemStatus.InProgress, "Ali");
                    job2Tasks[4].UpdateStatus(TaskItemStatus.Blocked, "Ali");
                    await dbContext.Tasks.AddRangeAsync(job2Tasks);
                    await dbContext.SaveChangesAsync();

                    // Update Job 2 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job2.Id, "Ali");

                    // Job 3: Marketing Campaign Launch
                    var job3 = new Job(domainUser.Id, "Spring Marketing Campaign Launch", "system");
                    await dbContext.Jobs.AddAsync(job3);
                    await dbContext.SaveChangesAsync();

                    var job3Tasks = new[]
                    {
                        new TaskItem(job3.Id, "Develop Campaign Strategy", "Create comprehensive marketing strategy including target demographics, channels, and key messaging", "system"),
                        new TaskItem(job3.Id, "Design Creative Assets", "Commission design team to produce promotional materials, social media graphics, and email templates", "system"),
                        new TaskItem(job3.Id, "Coordinate PR Activities", "Liaise with public relations agency to organise press releases and media engagement opportunities", "system"),
                        new TaskItem(job3.Id, "Implement Social Media Schedule", "Deploy scheduled posts across LinkedIn, Twitter, and Instagram platforms throughout campaign period", "system"),
                        new TaskItem(job3.Id, "Monitor Campaign Analytics", "Track engagement metrics, conversion rates, and return on investment using analytics platforms", "system")
                    };
                    // All tasks remain NotStarted
                    await dbContext.Tasks.AddRangeAsync(job3Tasks);
                    await dbContext.SaveChangesAsync();

                    // Update Job 3 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job3.Id, "Ali");

                    // Job 4: Office Relocation Project
                    var job4 = new Job(domainUser.Id, "Manchester Office Relocation Project", "system");
                    await dbContext.Jobs.AddAsync(job4);
                    await dbContext.SaveChangesAsync();

                    var job4Tasks = new[]
                    {
                        new TaskItem(job4.Id, "Source Suitable Premises", "Identify and evaluate potential office spaces within Manchester city centre meeting capacity requirements", "system"),
                        new TaskItem(job4.Id, "Negotiate Lease Agreement", "Finalise terms and conditions with landlord including rent, service charges, and break clauses", "system"),
                        new TaskItem(job4.Id, "Arrange Office Fit-Out", "Coordinate with contractors to install workstations, meeting rooms, and kitchen facilities", "system"),
                        new TaskItem(job4.Id, "Organise IT Infrastructure", "Ensure installation of network cabling, telecommunications, and server equipment prior to move date", "system"),
                        new TaskItem(job4.Id, "Plan Staff Relocation", "Communicate moving schedule to employees and arrange transport for equipment and personal belongings", "system"),
                        new TaskItem(job4.Id, "Update Business Address", "Notify Companies House, HMRC, clients, and suppliers of new registered office address", "system")
                    };
                    job4Tasks[0].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job4Tasks[1].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job4Tasks[2].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job4Tasks[3].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job4Tasks[4].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    job4Tasks[5].UpdateStatus(TaskItemStatus.Completed, "Ali");
                    await dbContext.Tasks.AddRangeAsync(job4Tasks);
                    await dbContext.SaveChangesAsync();

                    // Update Job 4 status based on tasks
                    await jobStatusService.UpdateJobStatusAsync(job4.Id, "Ali");
                }
            }
        } 
    }
}
