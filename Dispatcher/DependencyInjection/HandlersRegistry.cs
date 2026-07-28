using Dispatcher.Commands;
using Dispatcher.DomainEvents;
using Dispatcher.Middlewares;
using Dispatcher.Queries;
using Dispatcher.Requests;
using System.Reflection;

namespace Dispatcher.DependencyInjection;

public class HandlersRegistry
{
    public HandlersRegistry(IReadOnlyCollection<Assembly> assemblies, IReadOnlyCollection<Type> middlewareTypes)
    {
        IReadOnlyCollection<Type> allTypes = [.. assemblies.SelectMany(a => a.GetTypes())];

        this.QueryHandlerDefinitions = [.. allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleQuery<,>))
                .Select(i => new HandlerDefinition(i.GetGenericArguments().First(), i, t)))];

        this.CommandHandlerDefinitions = [.. allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IHandleCommand<>) || i.GetGenericTypeDefinition() == typeof(IHandleCommand<,>)))
                .Select(i => new HandlerDefinition(i.GetGenericArguments().First(), i, t)))];

        this.DomainEventHandlerDefinitions = [.. allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IListenToDomainEvent<>))
                .Select(i => new HandlerDefinition(i.GetGenericArguments().First(), i, t)))];

        this.MiddlewareHandlersDefinition = [.. middlewareTypes
            .SelectMany(m => m.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMiddleware<>))
                .Select(i => new HandlerDefinition(typeof(IRequest), i, m)))];
    }

    public IReadOnlyCollection<HandlerDefinition> QueryHandlerDefinitions { get; }
    public IReadOnlyCollection<HandlerDefinition> CommandHandlerDefinitions { get; }
    public IReadOnlyCollection<HandlerDefinition> DomainEventHandlerDefinitions { get; }
    public IReadOnlyCollection<HandlerDefinition> MiddlewareHandlersDefinition { get; }

    public IEnumerable<HandlerDefinition> AllDefinitions => [.. QueryHandlerDefinitions, .. CommandHandlerDefinitions, ..DomainEventHandlerDefinitions];
}

public class HandlerDefinition(Type handledType, Type handledInterface, Type handledConcreteType)
{
    public Type HandledType { get; } = handledType;
    public Type HandledInterface { get; } = handledInterface;
    public Type HandledConcreteType { get; } = handledConcreteType;
}