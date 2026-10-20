namespace RMediator.Abstractions;

/// <summary>Calls the next step of the pipeline: the following middleware or, at the end, the handler.</summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public delegate Task<TResponse> NextMiddleware<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

/// <summary>Calls the next step of the pipeline: the following middleware or, at the end, the handler.</summary>
public delegate Task NextMiddleware(IRequest request, CancellationToken cancellationToken);
