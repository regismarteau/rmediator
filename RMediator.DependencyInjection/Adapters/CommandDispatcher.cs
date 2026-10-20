using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal class CommandDispatcher(HandlersRegistry registry, IServiceProvider services) : DispatcherBase(registry, services), IDispatchCommand
{
    public async Task Dispatch(ICommand command, CancellationToken cancellationToken = default)
    {
        var middlewareServices = GetMiddlewareServicesFor(command);
        var handler = GetSingleHandler(command);

        await middlewareServices
            .Wrap((request, token) => Services.GetRequiredService(handler.InterfaceService).HandleGenericRequest(request, token))
            .Invoke(command, cancellationToken);
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return await SendRequest(command, cancellationToken);
    }
}
