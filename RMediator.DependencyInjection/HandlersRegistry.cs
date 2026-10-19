using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;
using System.Reflection;

namespace RMediator.DependencyInjection;

internal class HandlersRegistry(IReadOnlyCollection<Assembly> assemblies, IReadOnlyCollection<Type> middlewareTypes)
{
    private static readonly IReadOnlyCollection<Type> HandlerServiceTypes = [
        typeof(IHandleQuery<,>),
        typeof(IHandleCommand<>),
        typeof(IHandleCommand<,>),
        typeof(IListenToDomainEvent<>)];

    public IReadOnlyCollection<HandlerDefinition> MiddlewareHandlersDefinition { get; } = [.. middlewareTypes
            .SelectMany(m => m.GetInterfaces()
                .Where(i => i.IsAMiddleware())
                .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, m)))];

    public IEnumerable<HandlerDefinition> HandlerDefinitions { get; } = [.. assemblies
            .SelectMany(a => a.GetTypes())
            .SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsOfGenericType([.. HandlerServiceTypes]))
            .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, t)))];

    public int GetMiddlewareOrder(Type middlewareType)
    {
        var concreteType = middlewareType.IsGenericType ? middlewareType.GetGenericTypeDefinition() : middlewareType;
        var index = MiddlewareHandlersDefinition.Select(d => d.ConcreteType).ToList().IndexOf(concreteType);
        return index < 0 ? int.MaxValue : index;
    }

    public HandlersRegistry Merge(IReadOnlyCollection<Assembly> otherAssemblies, IReadOnlyCollection<Type> otherMiddlewareTypes)
    {
        return new([.. assemblies.Concat(otherAssemblies).Distinct()], [.. middlewareTypes.Concat(otherMiddlewareTypes).Distinct()]);
    }
}


internal class HandlerDefinition(Type handledType, Type interfaceService, Type concreteType)
{
    public Type HandledType { get; } = handledType;
    public Type InterfaceService { get; } = interfaceService;
    public Type RelativeToGenericConcreteTypeInterfaceService { get; } = concreteType.IsGenericType && interfaceService.IsGenericType ? interfaceService.GetGenericTypeDefinition() : interfaceService;
    public Type ConcreteType { get; } = concreteType;
}