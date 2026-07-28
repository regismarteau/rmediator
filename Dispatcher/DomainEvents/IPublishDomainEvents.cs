using Dispatcher.Requests;

namespace Dispatcher.DomainEvents;

public interface IPublishDomainEvents
{
    Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken? cancellationToken = null) where TDomainEvent : IDomainEvent;
}

public interface IDomainEvent : IRequest;

public interface IListenToDomainEvent<in TDomainEvent> : IHandleRequest where TDomainEvent : IDomainEvent
{
    Task Listen(TDomainEvent @event, CancellationToken cancellationToken);

    async Task IHandleRequest.Handle(IRequest request, CancellationToken cancellationToken)
    {
        await this.Listen((TDomainEvent)request, cancellationToken);
    }
}