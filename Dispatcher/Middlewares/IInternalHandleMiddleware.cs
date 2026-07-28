using Dispatcher.Requests;

namespace Dispatcher.Middlewares;

public interface IHandleMiddleware<in TRequest, TResponse> : IInternalHandleMiddleware<TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);

    Task<TResponse> IInternalHandleMiddleware<TResponse>.Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        return this.Handle((TRequest)request, next, cancellationToken);
    }
}

public interface IHandleMiddleware<in TRequest> : IInternalHandleMiddleware where TRequest : IRequest
{
    Task Handle(TRequest request, NextMiddleware next, CancellationToken cancellationToken);

    Task IInternalHandleMiddleware.Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken)
    {
        return this.Handle((TRequest)request, next, cancellationToken);
    }
}

public interface IInternalHandleMiddleware<TResponse>
{
    Task<TResponse> Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);
}

public interface IInternalHandleMiddleware
{
    Task Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> NextMiddleware<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

public delegate Task NextMiddleware(IRequest request, CancellationToken cancellationToken);