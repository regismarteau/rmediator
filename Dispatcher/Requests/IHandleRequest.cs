namespace Dispatcher.Requests;

public interface IHandleRequest<TResponse>
{
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
}
