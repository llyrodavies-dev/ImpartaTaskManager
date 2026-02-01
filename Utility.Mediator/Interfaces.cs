namespace Utility.Mediator;

/// <summary>
/// Defines a mediator that sends requests to their corresponding handlers and returns a response asynchronously.
/// </summary>
/// <remarks>The mediator pattern decouples the sender of a request from its handler, allowing for flexible and
/// maintainable request/response processing. Implementations of this interface are typically used to coordinate
/// application logic by dispatching requests to the appropriate handlers. This interface is thread-safe if implemented
/// accordingly.</remarks>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
/// <param name="request">The request to be sent to the handler.</param>
/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
/// <returns>A  <see cref="Task{TResponse}"/> representing the asynchronous operation.</returns>
public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a request that expects a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response expected.</typeparam>
public interface IRequest<TResponse> { }

/// <summary>
/// Defines a handler for processing a request and producing a response asynchronously.
/// </summary>
/// <remarks>Implement this interface to define the logic for handling a specific request type. Handlers are
/// typically used in request/response messaging patterns to decouple request processing from the request sender.
/// Implementations should be thread-safe if they are intended to be used concurrently.</remarks>
/// <typeparam name="TRequest">The type of request to handle. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a behavior that can be executed before and after a request is handled in a processing pipeline.
/// </summary>
/// <remarks>Implement this interface to add custom logic, such as logging, validation, or performance monitoring,
/// to the request handling pipeline. Pipeline behaviors are executed in the order they are registered and can modify or
/// short-circuit the handling of requests.</remarks>
/// <typeparam name="TRequest">The type of the request message handled by the pipeline behavior. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
