namespace HumbleMediator.Abstractions;

public interface IPublishDomainEvent
{
    Task Publish(IDomainEvent @event, CancellationToken? cancellationToken = null);
}

public interface IDomainEvent : IRequest;

public interface IListenToDomainEvent<in TDomainEvent> : IHandleRequest<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Listen(TDomainEvent @event, CancellationToken cancellationToken);

    async Task IHandleRequest<TDomainEvent>.Handle(TDomainEvent request, CancellationToken cancellationToken)
    {
        await Listen(request, cancellationToken);
    }
}