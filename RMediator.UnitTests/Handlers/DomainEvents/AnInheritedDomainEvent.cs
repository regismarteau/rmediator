using RMediator.Abstractions;

namespace RMediator.UnitTests.Handlers.DomainEvents;

public record AnInheritedDomainEvent : AParentTypeDomainEvent;
public record AParentTypeDomainEvent : ABaseTypeDomainEvent;
public record ABaseTypeDomainEvent : IAmASpecificDomainEvent;

public interface IAmASpecificDomainEvent : IDomainEvent;

public class AFirstStronglyTypedTwiceListenerHandler(DomainEventListened domainEvents) : IListenToDomainEvent<AnInheritedDomainEvent>, IListenToDomainEvent<AParentTypeDomainEvent>
{
    public Task Listen(AnInheritedDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }

    public Task Listen(AParentTypeDomainEvent @event, CancellationToken cancellationToken)
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

public class AFourthGenericListenerHandler<TDomainEvent>(DomainEventListened domainEvents) : IListenToDomainEvent<TDomainEvent> where TDomainEvent : ABaseTypeDomainEvent
{
    public Task Listen(TDomainEvent @event, CancellationToken cancellationToken)
    {
        domainEvents.Listen(GetType());
        return Task.CompletedTask;
    }
}