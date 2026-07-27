using Dispatcher.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.DomainEvents;

public class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : IPublishDomainEvents
{
    public async Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken? cancellationToken = null) where TDomainEvent : IDomainEvent
    {
        IReadOnlyCollection<HandlerDefinition> handlers = [.. registry.DomainEventHandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(@event.GetType()))];
        if (handlers.Count == 0)
        {
            throw new InvalidOperationException($"There is no listener for event {@event.GetType().Name}");
        }

        foreach (var handler in handlers)
        {
            await ((IListenToDomainEvent<TDomainEvent>)services.GetRequiredService(handlers.First().HandledInterface)).Listen(@event, cancellationToken ?? CancellationToken.None);
        }
    }
}