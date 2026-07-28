using Dispatcher.Requests;

namespace Dispatcher.Middlewares;

public interface IHandleMiddleware<in TRequest, TResponse> : IHandleMiddleware<TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);

    Task<TResponse> IHandleMiddleware<TResponse>.Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        return this.Handle((TRequest)request, next, cancellationToken);
    }
}

public interface IHandleMiddleware<TResponse>
{
    Task<TResponse> Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> NextMiddleware<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);