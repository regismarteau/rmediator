namespace RMediator.Abstractions;

/// <summary>Publishes domain events to their listeners.</summary>
public interface IPublishDomainEvent
{
    /// <summary>Publishes an event. Listeners run one after the other and the first exception is thrown to the caller. Does nothing if nobody listens.</summary>
    /// <param name="event">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task Publish(IDomainEvent @event, CancellationToken cancellationToken = default);
}

/// <summary>Marker for something that happened and may have any number of listeners.</summary>
public interface IDomainEvent : IRequest;

/// <summary>Listens to a domain event. A listener of a base type also receives its sub-types.</summary>
/// <typeparam name="TDomainEvent">The type of event.</typeparam>
public interface IListenToDomainEvent<in TDomainEvent> : IHandleRequest<TDomainEvent> where TDomainEvent : IDomainEvent
{
    /// <summary>Reacts to the event.</summary>
    /// <param name="event">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task Listen(TDomainEvent @event, CancellationToken cancellationToken);

    async Task IHandleRequest<TDomainEvent>.Handle(TDomainEvent request, CancellationToken cancellationToken)
    {
        await Listen(request, cancellationToken);
    }
}
