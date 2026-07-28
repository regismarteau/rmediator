namespace Dispatcher.Abstractions;

public delegate Task<TResponse> NextMiddleware<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

public delegate Task NextMiddleware(IRequest request, CancellationToken cancellationToken);