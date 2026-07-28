using Dispatcher.DomainEvents;
using Dispatcher.Middlewares;
using Dispatcher.UnitTests.Handlers.Middlewares;

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

public class AThirdlyExecutedStronglyTypedDomainEventMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<SomeDomainEvent>
{
    public async Task Handle(SomeDomainEvent request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}