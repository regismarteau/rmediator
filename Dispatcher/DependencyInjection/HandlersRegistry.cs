using Dispatcher.Commands;
using Dispatcher.DomainEvents;
using Dispatcher.Queries;
using System.Reflection;

namespace Dispatcher.DependencyInjection;

public class HandlersRegistry
{
    public HandlersRegistry(IReadOnlyCollection<Assembly> assemblies)
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
                .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IListenToDomainEvent<>)))
                .Select(i => new HandlerDefinition(i.GetGenericArguments().First(), i, t)))];
    }

    public IReadOnlyCollection<HandlerDefinition> QueryHandlerDefinitions { get; }
    public IReadOnlyCollection<HandlerDefinition> CommandHandlerDefinitions { get; }
    public IReadOnlyCollection<HandlerDefinition> DomainEventHandlerDefinitions { get; }

    public IEnumerable<HandlerDefinition> AllDefinitions => [.. QueryHandlerDefinitions, .. CommandHandlerDefinitions, ..DomainEventHandlerDefinitions];
}

public class HandlerDefinition(Type handledType, Type handledInterface, Type handledConcreteType)
{
    public Type HandledType { get; } = handledType;
    public Type HandledInterface { get; } = handledInterface;
    public Type HandledConcreteType { get; } = handledConcreteType;
}