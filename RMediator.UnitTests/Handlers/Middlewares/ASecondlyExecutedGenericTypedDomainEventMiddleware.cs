using RMediator.Abstractions;

namespace RMediator.UnitTests.Handlers.Middlewares;

public class ASecondlyExecutedGenericTypedDomainEventMiddleware<TDomainEvent>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TDomainEvent> where TDomainEvent : IDomainEvent
{
    public async Task Handle(TDomainEvent request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}