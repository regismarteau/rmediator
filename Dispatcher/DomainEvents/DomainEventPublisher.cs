using Dispatcher.DependencyInjection;
using Dispatcher.Middlewares;
using Dispatcher.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.DomainEvents;

public class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IPublishDomainEvents
{
    public async Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken? cancellationToken = null) where TDomainEvent : IDomainEvent
    {
        IReadOnlyCollection<HandlerDefinition> handlers = GetHandlersFor(@event);
        if (handlers.Count == 0)
        {
            throw new InvalidOperationException($"There is no listener for event {@event.GetType().Name}");
        }

        var middlewareServices = GetMiddlewareFor(@event);

        if (middlewareServices.Count == 0)
        {
            await ExecuteHandlers(@event, handlers, cancellationToken);
            return;
        }

        await middlewareServices.First().Handle(@event, GetNextMiddleware<TDomainEvent>(middlewareServices, handlers, 0), cancellationToken ?? CancellationToken.None);
    }


    private async Task ExecuteHandlers<TDomainEvent>(TDomainEvent @event, IReadOnlyCollection<HandlerDefinition> handlers, CancellationToken? cancellationToken) where TDomainEvent : IDomainEvent
    {
        foreach (var handler in handlers.SelectMany(h => services
            .GetServices(h.HandledInterface))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType()))
        {
            await ((IListenToDomainEvent<TDomainEvent>)handler!).Listen(@event, cancellationToken ?? CancellationToken.None);
        }
    }

    private NextMiddleware GetNextMiddleware<TDomainEvent>(IReadOnlyCollection<IInternalHandleMiddleware> middlewares, IReadOnlyCollection<HandlerDefinition> handlers, int index) where TDomainEvent : IDomainEvent
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ExecuteHandlers((TDomainEvent)request, handlers, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware<TDomainEvent>(middlewares, handlers, index + 1), token);
    }
}