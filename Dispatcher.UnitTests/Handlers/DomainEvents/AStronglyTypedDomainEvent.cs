using Dispatcher.DomainEvents;

namespace Dispatcher.UnitTests.Handlers.DomainEvents;

public record AStronglyTypedDomainEvent : ABaseTypeDomainEvent;

public record ABaseTypeDomainEvent : IAmASpecificDomainEvent;

public interface IAmASpecificDomainEvent : IDomainEvent;

public class AFirstStronglyTypedListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<AStronglyTypedDomainEvent>
{
    public Task Listen(AStronglyTypedDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}

public class ASecondLessStrictConcreteTypeListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<ABaseTypeDomainEvent>
{
    public Task Listen(ABaseTypeDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}

public class AThirdInheritedInterfaceListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<IAmASpecificDomainEvent>
{
    public Task Listen(IAmASpecificDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}