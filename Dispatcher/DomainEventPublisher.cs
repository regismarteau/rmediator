using Dispatcher.Abstractions;
using Dispatcher.DependencyInjection;
using Dispatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher;

internal class DomainEventPublisher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IPublishDomainEvents
{
    private readonly IServiceProvider services = services;

    public async Task Publish(IDomainEvent @event, CancellationToken? cancellationToken = null)
    {
        var handlers = GetHandlersFor(@event)
            .DistinctBy(handler => handler.InterfaceService)
            .SelectMany(handler => services
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

        var middlewareServices = GetMiddlewareServicesFor(@event)
            .Wrap((request, token) => handlers.ForEachAsync(handler => handler.HandleGenericRequest(request, token)))
            .Invoke(@event, cancellationToken ?? CancellationToken.None);
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