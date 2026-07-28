using Dispatcher.Commands;
using Dispatcher.DomainEvents;
using Dispatcher.Extensions;
using Dispatcher.Middlewares;
using Dispatcher.Queries;
using System.Reflection;

namespace Dispatcher.DependencyInjection;

public class HandlersRegistry(IReadOnlyCollection<Assembly> assemblies, IReadOnlyCollection<Type> middlewareTypes)
{
    private static readonly IReadOnlyCollection<Type> HandlerServiceTypes = [
        typeof(IHandleQuery<,>),
        typeof(IHandleCommand<>),
        typeof(IHandleCommand<,>),
        typeof(IListenToDomainEvent<>)];

    public IReadOnlyCollection<HandlerDefinition> MiddlewareHandlersDefinition { get; } = [.. middlewareTypes
            .SelectMany(m => m.GetInterfaces()
                .Where(i => i.IsOfGenericType(typeof(IHandleMiddleware<,>), typeof(IHandleMiddleware<>)))
                .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, m)))];

    public IEnumerable<HandlerDefinition> HandlerDefinitions { get; } = [.. assemblies
            .SelectMany(a => a.GetTypes())
            .SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsOfGenericType([.. HandlerServiceTypes]))
            .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, t)))];
}

public class HandlerDefinition(Type handledType, Type handledInterface, Type handledConcreteType)
{
    public Type HandledType { get; } = handledType;
    public Type HandledInterface { get; } = handledInterface;
    public Type HandledConcreteType { get; } = handledConcreteType;
}