namespace HumbleMediator.Abstractions;

public interface IAbstractMiddlewareHandler<TResponse>
{
    Task<TResponse> Handle(IRequest<TResponse> request, NextMiddleware<TResponse> next, CancellationToken cancellationToken);
}

public interface IAbstractMiddlewareHandler
{
    Task Handle(IRequest request, NextMiddleware next, CancellationToken cancellationToken);
}