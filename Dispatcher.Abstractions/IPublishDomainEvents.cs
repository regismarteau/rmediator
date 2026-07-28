namespace Dispatcher.Abstractions;

public interface IPublishDomainEvents
{
    Task Publish(IDomainEvent @event, CancellationToken? cancellationToken = null);
}

public interface IDomainEvent : IRequest;

public interface IListenToDomainEvent<in TDomainEvent> : IHandleRequest where TDomainEvent : IDomainEvent
{
    Task Listen(TDomainEvent @event, CancellationToken cancellationToken);

    async Task IHandleRequest.Handle(IRequest request, CancellationToken cancellationToken)
    {
        await Listen((TDomainEvent)request, cancellationToken);
    }
}