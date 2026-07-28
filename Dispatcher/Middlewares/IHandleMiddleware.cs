using Dispatcher.Requests;

namespace Dispatcher.Middlewares;

public interface IHandleMiddleware<TResponse>
{
    Task<TResponse> Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> NextMiddleware<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);