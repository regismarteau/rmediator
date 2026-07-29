namespace Dispatcher.Abstractions;

public interface IHandleRequest<in TRequest> where TRequest : IRequest
{
    public Task Handle(TRequest request, CancellationToken cancellationToken);
}


public interface IHandleRequestWithResponse<TResponse>
{
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
}
