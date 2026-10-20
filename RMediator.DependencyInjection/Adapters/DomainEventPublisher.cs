using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : DispatcherBase(registry, services), IPublishDomainEvent
{
    public async Task Publish(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var handlers = GetHandlersFor(@event)
            .DistinctBy(handler => handler.InterfaceService)
            .SelectMany(handler => Services
                .GetServices(handler.InterfaceService)
                .Where(service => service is not null)
                .Select(service => new { Handler = handler, Service = service }))
            .DistinctBy(unique => new UniqueHandlerPerTypeAndSubType(unique.Handler.InterfaceService, unique.Service!.GetType()))
            .Select(unique => unique.Service!)
            .ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        await GetMiddlewareServicesFor(@event)
            .Wrap((request, token) => handlers.ForEachAsync(handler => handler.HandleGenericRequest(request, token)))
            .Invoke(@event, cancellationToken);
    }

    private record UniqueHandlerPerTypeAndSubType
    {
        public UniqueHandlerPerTypeAndSubType(Type interfaceServiceType, Type handlerType)
        {
            InterfaceServiceType = handlerType.IsGenericType ? interfaceServiceType.GetGenericTypeDefinition() : interfaceServiceType;
            HandlerType = handlerType.IsGenericType ? handlerType.GetGenericTypeDefinition() : handlerType;
        }

        public Type InterfaceServiceType { get; }
        public Type HandlerType { get; }
    }
}
