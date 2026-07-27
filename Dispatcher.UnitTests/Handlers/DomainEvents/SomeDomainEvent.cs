using Dispatcher.DomainEvents;

namespace Dispatcher.UnitTests.Handlers.DomainEvents;

public record SomeDomainEvent(string RaisedBy) : IDomainEvent;

public class SomeDomainEventListener(DomainEventListened domainEvents) : IListenToDomainEvent<SomeDomainEvent>
{
    public Task Listen(SomeDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.RaiseBy(@event.RaisedBy);
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}