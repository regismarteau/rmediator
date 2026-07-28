using Dispatcher.Requests;

namespace Dispatcher.DependencyInjection;

public interface IHandleRequest<TResponse>
{
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
}
