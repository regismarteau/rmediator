namespace Dispatcher.DomainEvents;

public interface IPublishDomainEvents
{
    Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken? cancellationToken = null) where TDomainEvent : IDomainEvent;
}

public interface IDomainEvent;

public interface IListenToDomainEvent<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Listen(TDomainEvent @event, CancellationToken cancellationToken);
}