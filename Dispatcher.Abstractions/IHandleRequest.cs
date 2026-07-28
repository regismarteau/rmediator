namespace Dispatcher.Abstractions;

public interface IHandleRequest
{
    public Task Handle(IRequest request, CancellationToken cancellationToken);
}


public interface IHandleRequest<TResponse>
{
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
}
