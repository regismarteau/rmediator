using HumbleMediator.Abstractions;
using HumbleMediator.DependencyInjection;
using HumbleMediator.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HumbleMediator;

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