using Dispatcher.DomainEvents;

namespace Dispatcher.UnitTests.Handlers.DomainEvents;

public record AnInheritedDomainEvent : ABaseTypeDomainEvent;

public record ABaseTypeDomainEvent : IAmASpecificDomainEvent;

public interface IAmASpecificDomainEvent : IDomainEvent;

public class AFirstStronglyTypedListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<AnInheritedDomainEvent>
{
    public Task Listen(AnInheritedDomainEvent @event, CancellationToken cancellationToken)
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