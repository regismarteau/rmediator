using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal class CommandDispatcher(HandlersRegistry registry, IServiceProvider services) : Mediator(registry, services), IDispatchCommand
{
    private readonly IServiceProvider services = services;

    public async Task Dispatch(ICommand command, CancellationToken? cancellationToken = null)
    {
        var middlewareServices = GetMiddlewareServicesFor(command);
        var handler = GetSingleHandler(command);

        await middlewareServices
            .Wrap((request, token) => services.GetRequiredService(handler.InterfaceService).HandleGenericRequest(request, token))
            .Invoke(command, cancellationToken ?? CancellationToken.None);
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null)
    {
        return await SendRequest(command, cancellationToken ?? CancellationToken.None);
    }
}