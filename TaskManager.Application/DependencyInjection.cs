using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Services;
using TaskManager.Domain.Projections;
using Utility.Filtering;
using Utility.Mediator;

namespace TaskManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            #region Filter Configuratoin
            services.AddSingleton<FilterModelConfiguration<TaskItemProjection>>();
            #endregion

            // application services
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            services.AddScoped<IJobStatusService, JobStatusService>();

            // Register validators, behaviors
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
            return services;
        }
    }
}
