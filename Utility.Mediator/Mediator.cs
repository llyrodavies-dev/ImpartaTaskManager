using Microsoft.Extensions.DependencyInjection;

namespace Utility.Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends a request to its corresponding handler and returns the response.
        /// </summary>
        /// <typeparam name="TResponse">The type of response expected from the handler.</typeparam>
        /// <param name="request">The request to be processed.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The response from the request handler.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the request type.</exception>
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            dynamic handler = _serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"No handler found for request of type {request.GetType().FullName}");

            var pipelineBehaviouType = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            IEnumerable<object?> behaviorList = _serviceProvider.GetServices(pipelineBehaviouType).Reverse().Cast<dynamic>();

            Func<Task<TResponse>> handlerDelegate = () => handler.Handle((dynamic)request, cancellationToken);

            if (behaviorList != null)
            {
                foreach (dynamic? behavior in behaviorList.Reverse())
                {
                    if(behavior == null) continue;
                    Func<Task<TResponse>> next = handlerDelegate;
                    handlerDelegate = () => behavior.Handle((dynamic)request, next, cancellationToken);
                }
            }

            return await handlerDelegate();
        }

        /// <summary>
        /// Publishes a notification to all registered handlers.
        /// </summary>
        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            Type notificationType = notification.GetType();
            Type handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

            // Get all handlers for this notification
            var handlers = _serviceProvider.GetServices(handlerType).Cast<dynamic>();

            // Execute all handlers concurrently
            var tasks = new List<Task>();
            foreach (dynamic handler in handlers)
            {
                tasks.Add(handler.Handle((dynamic)notification, cancellationToken));
            }

            // Wait for all handlers to complete
            await Task.WhenAll(tasks);
        }
    }
}
