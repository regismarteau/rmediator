using HumbleMediator.Abstractions;

namespace HumbleMediator.UnitTests.Handlers.DomainEvents;

public record AMultipleListenedDomainEvent() : IDomainEvent;

public class AFirstListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<AMultipleListenedDomainEvent>
{
    public Task Listen(AMultipleListenedDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}

public class ASecondListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<AMultipleListenedDomainEvent>
{
    public Task Listen(AMultipleListenedDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}