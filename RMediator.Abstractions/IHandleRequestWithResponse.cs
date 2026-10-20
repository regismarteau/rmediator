namespace RMediator.Abstractions;

/// <summary>Handles a request without response. Implement <see cref="IHandleCommand{TCommand}"/> or <see cref="IListenToDomainEvent{TDomainEvent}"/> instead.</summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
public interface IHandleRequest<in TRequest> where TRequest : IRequest
{
    /// <summary>Handles the request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task Handle(TRequest request, CancellationToken cancellationToken);
}


/// <summary>Handles a request with a response. Implement <see cref="IHandleCommand{TCommand, TResult}"/> or <see cref="IHandleQuery{TQuery, TResponse}"/> instead.</summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IHandleRequestWithResponse<TResponse>
{
    /// <summary>Handles the request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
}
