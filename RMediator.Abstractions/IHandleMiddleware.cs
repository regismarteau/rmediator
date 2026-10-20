namespace RMediator.Abstractions;

/// <summary>A pipeline step wrapping the handler of requests that produce a response (queries and commands with a result). Middlewares run in the order they are registered.</summary>
/// <typeparam name="TRequest">The type of request this middleware applies to, including its sub-types.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IHandleMiddleware<in TRequest, TResponse> : IAbstractMiddlewareHandler<TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>Runs the middleware. Call <paramref name="next"/> to continue the pipeline.</summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The next step of the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);

    Task<TResponse> IAbstractMiddlewareHandler<TResponse>.Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        return Handle((TRequest)request, next, cancellationToken);
    }
}

/// <summary>A pipeline step wrapping the handler of requests without response (commands without a result and domain events). Middlewares run in the order they are registered.</summary>
/// <typeparam name="TRequest">The type of request this middleware applies to, including its sub-types.</typeparam>
public interface IHandleMiddleware<in TRequest> : IAbstractMiddlewareHandler where TRequest : IRequest
{
    /// <summary>Runs the middleware. Call <paramref name="next"/> to continue the pipeline.</summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The next step of the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task Handle(TRequest request, NextMiddleware next, CancellationToken cancellationToken);

    Task IAbstractMiddlewareHandler.Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken)
    {
        return Handle((TRequest)request, next, cancellationToken);
    }
}
