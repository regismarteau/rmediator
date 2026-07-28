using Dispatcher.DependencyInjection;
using Dispatcher.Extensions;
using Dispatcher.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.DomainEvents;

internal class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IPublishDomainEvents
{
    private readonly IServiceProvider services = services;

    public async Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken? cancellationToken = null) where TDomainEvent : IDomainEvent
    {
        IReadOnlyCollection<HandlerDefinition> handlers = GetHandlersFor(@event);
        if (handlers.Count == 0)
        {
            throw new InvalidOperationException($"There is no listener for event {@event.GetType().Name}");
        }

        var middlewareServices = GetMiddlewareServicesFor(@event)
            .Wrap((request, token) => ExecuteHandlers((TDomainEvent)request, handlers, token))
            .Invoke(@event, cancellationToken ?? CancellationToken.None);
    }


    private async Task ExecuteHandlers<TDomainEvent>(TDomainEvent @event, IReadOnlyCollection<HandlerDefinition> handlers, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
    {
        foreach (var handler in handlers.SelectMany(h => services
            .GetServices(h.HandledInterface))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType()))
        {
            await ((IListenToDomainEvent<TDomainEvent>)handler!).Listen(@event, cancellationToken);
        }
    }
}