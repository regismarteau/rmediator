namespace RMediator.Abstractions;

/// <summary>Non-generic view of a middleware with a response, used by the mediator. Implement <see cref="IHandleMiddleware{TRequest, TResponse}"/> instead.</summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IAbstractMiddlewareHandler<TResponse>
{
    /// <inheritdoc cref="IHandleMiddleware{TRequest, TResponse}.Handle(TRequest, NextMiddleware{TResponse}, CancellationToken)"/>
    Task<TResponse> Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>Non-generic view of a middleware without a response, used by the mediator. Implement <see cref="IHandleMiddleware{TRequest}"/> instead.</summary>
public interface IAbstractMiddlewareHandler
{
    /// <inheritdoc cref="IHandleMiddleware{TRequest}.Handle(TRequest, NextMiddleware, CancellationToken)"/>
    Task Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken);
}
