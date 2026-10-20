namespace RMediator.Abstractions;

/// <summary>Dispatches queries to their single handler through the middleware pipeline.</summary>
public interface IDispatchQuery
{
    /// <summary>Dispatches a query.</summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response of the handler.</returns>
    /// <exception cref="InvalidOperationException">No handler, or several handlers, are registered.</exception>
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}

/// <summary>A request that reads state without changing it.</summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>;

/// <summary>Handles a query. A query must have exactly one handler.</summary>
/// <typeparam name="TQuery">The type of query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IHandleQuery<in TQuery, TResponse> : IHandleRequestWithResponse<TResponse> where TQuery : IQuery<TResponse>
{
    /// <summary>Handles the query.</summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);

    async Task<TResponse> IHandleRequestWithResponse<TResponse>.Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        return await Handle((TQuery)request, cancellationToken);
    }
}
