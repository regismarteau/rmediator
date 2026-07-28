using Dispatcher.Requests;

namespace Dispatcher.Middlewares;

public interface IHandleMiddleware<in TRequest, TResponse> : IAbstractMiddlewareHandler<TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);

    Task<TResponse> IAbstractMiddlewareHandler<TResponse>.Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        return this.Handle((TRequest)request, next, cancellationToken);
    }
}

public interface IHandleMiddleware<in TRequest> : IAbstractMiddlewareHandler where TRequest : IRequest
{
    Task Handle(TRequest request, NextMiddleware next, CancellationToken cancellationToken);

    Task IAbstractMiddlewareHandler.Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken)
    {
        return this.Handle((TRequest)request, next, cancellationToken);
    }
}