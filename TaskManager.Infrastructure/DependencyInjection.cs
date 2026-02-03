using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Application.Common.Configuration;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Identity;
using TaskManager.Infrastructure.Interfaces;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;
using AuthService = TaskManager.Infrastructure.Identity.AuthService;

namespace TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false, // Simplified - no audience validation
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

            // HTTP Context Accessor
            services.AddHttpContextAccessor();

            // Applicaiton layer interfaces
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITaskItemRepository, TaskItemRepository>();

            // Domain layer interfaces
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Infrastructure layer interfaces
            services.AddScoped<ITokenService, TokenService>();

            // Configure FileStorage options (defined in Application layer)
            services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

            // Register FileStorageService with base path from configuration
            services.AddSingleton<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}
