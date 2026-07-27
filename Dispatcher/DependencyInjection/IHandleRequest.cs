namespace Dispatcher.DependencyInjection;

public interface IHandleRequest
{
    public Task<object> Handle(object request, CancellationToken cancellationToken);
}
