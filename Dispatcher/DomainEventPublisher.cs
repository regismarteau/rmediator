using Dispatcher.Abstractions;
using Dispatcher.DependencyInjection;
using Dispatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher;

internal class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IPublishDomainEvents
{
    private readonly IServiceProvider services = services;

    public async Task Publish(IDomainEvent @event, CancellationToken? cancellationToken = null)
    {
        IReadOnlyCollection<HandlerDefinition> handlers = GetHandlersFor(@event);
        if (handlers.Count == 0)
        {
            return;
        }

        var middlewareServices = GetMiddlewareServicesFor(@event)
            .Wrap((request, token) => ExecuteHandlers((IDomainEvent)request, handlers, token))
            .Invoke(@event, cancellationToken ?? CancellationToken.None);
    }


    private async Task ExecuteHandlers(IDomainEvent @event, IReadOnlyCollection<HandlerDefinition> handlers, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers.SelectMany(h => services
            .GetServices(h.InterfaceService))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType().IsGenericType ? h.GetType().GetGenericTypeDefinition() : h.GetType()))
        {
            await ((IHandleRequest)handler!).Handle(@event, cancellationToken);
        }
    }
}