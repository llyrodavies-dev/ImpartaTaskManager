using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                foreach (dynamic behavior in behaviorList.Reverse())
                {
                    Func<Task<TResponse>> next = handlerDelegate;
                    handlerDelegate = () => behavior.Handle((dynamic)request, next, cancellationToken);
                }
            }

            return await handlerDelegate();
        }
    }
}
