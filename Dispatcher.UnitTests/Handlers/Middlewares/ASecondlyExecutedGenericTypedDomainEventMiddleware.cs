using Dispatcher.DomainEvents;
using Dispatcher.Middlewares;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class ASecondlyExecutedGenericTypedDomainEventMiddleware<TDomainEvent>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TDomainEvent> where TDomainEvent : IDomainEvent
{
    public async Task Handle(TDomainEvent request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}