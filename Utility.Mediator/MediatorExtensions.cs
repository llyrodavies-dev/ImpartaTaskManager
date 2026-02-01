using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Utility.Mediator
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddScoped<IMediator, Mediator>();
            return services;
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<IMediator, Mediator>();

            Type hanlerInterfaceType = typeof(IRequestHandler<,>);
            Type notificationHandlerInterfaceType = typeof(INotificationHandler<>);

            foreach (Assembly assembly in assemblies)
            {
                // Register request handlers
                var handlerTypes = assembly
                    .GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(type => type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == hanlerInterfaceType)
                        .Select(i => new { Interface = i, implementation = type }));

                foreach (var handler in handlerTypes)
                {
                    services.AddTransient(handler.Interface, handler.implementation);
                }

                // Register notification handlers
                var notificationHandlerTypes = assembly
                    .GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(type => type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == notificationHandlerInterfaceType)
                        .Select(i => new { Interface = i, Implementation = type }));

                foreach (var handler in notificationHandlerTypes)
                {
                    services.AddScoped(handler.Interface, handler.Implementation);
                }
            }

            return services;
        }
    }
}
