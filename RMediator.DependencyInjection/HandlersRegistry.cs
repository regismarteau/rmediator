using System.Collections.Concurrent;
using System.Reflection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection;

internal class HandlersRegistry(IReadOnlyCollection<Assembly> assemblies, IReadOnlyCollection<Type> middlewareTypes)
{
    private static readonly IReadOnlyCollection<Type> HandlerServiceTypes = [
        typeof(IHandleQuery<,>),
        typeof(IHandleCommand<>),
        typeof(IHandleCommand<,>),
        typeof(IListenToDomainEvent<>)];

    private readonly ConcurrentDictionary<Type, IReadOnlyCollection<HandlerDefinition>> handlersByRequestType = new();
    private readonly ConcurrentDictionary<(Type RequestType, Type? ResponseType), IReadOnlyCollection<HandlerDefinition>> baseTypeMiddlewares = new();

    public IReadOnlyCollection<HandlerDefinition> MiddlewareHandlersDefinition { get; } = [.. middlewareTypes
            .SelectMany(m => m.GetInterfaces()
                .Where(i => i.IsAMiddleware())
                .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, m)))];

    public IReadOnlyCollection<HandlerDefinition> HandlerDefinitions { get; } = [.. assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsOfGenericType([.. HandlerServiceTypes]))
            .Select(i => new HandlerDefinition(i.GetGenericArguments()[0], i, t)))];

    private IReadOnlyDictionary<Type, int> MiddlewareOrders => middlewareOrders ??= MiddlewareHandlersDefinition
        .Select((definition, index) => (definition.ConcreteType, index))
        .GroupBy(entry => entry.ConcreteType)
        .ToDictionary(group => group.Key, group => group.First().index);

    private IReadOnlyDictionary<Type, int>? middlewareOrders;

    public int GetMiddlewareOrder(Type middlewareType)
    {
        var concreteType = middlewareType.IsGenericType ? middlewareType.GetGenericTypeDefinition() : middlewareType;
        return MiddlewareOrders.GetValueOrDefault(concreteType, int.MaxValue);
    }

    public IReadOnlyCollection<HandlerDefinition> GetHandlersFor(Type requestType)
    {
        return handlersByRequestType.GetOrAdd(requestType, type => [.. HandlerDefinitions.Where(d => d.HandledType.IsAssignableFrom(type))]);
    }

    public IReadOnlyCollection<HandlerDefinition> GetBaseTypeMiddlewaresFor(Type requestType, Type? responseType)
    {
        return baseTypeMiddlewares.GetOrAdd((requestType, responseType), key => [.. MiddlewareHandlersDefinition
            .Where(d => !d.ConcreteType.IsGenericTypeDefinition
                && d.HandledType != key.RequestType
                && d.HandledType.IsAssignableFrom(key.RequestType)
                && IsMiddlewareServiceFor(d.InterfaceService, key.ResponseType))
            .DistinctBy(d => d.InterfaceService)]);
    }

    public HandlersRegistry Merge(IReadOnlyCollection<Assembly> otherAssemblies, IReadOnlyCollection<Type> otherMiddlewareTypes)
    {
        return new([.. assemblies.Concat(otherAssemblies).Distinct()], [.. middlewareTypes.Concat(otherMiddlewareTypes).Distinct()]);
    }

    private static bool IsMiddlewareServiceFor(Type middlewareService, Type? responseType)
    {
        return responseType is null
            ? middlewareService.GetGenericTypeDefinition() == typeof(IHandleMiddleware<>)
            : middlewareService.GetGenericTypeDefinition() == typeof(IHandleMiddleware<,>)
                && middlewareService.GetGenericArguments()[1] == responseType;
    }
}

internal class HandlerDefinition(Type handledType, Type interfaceService, Type concreteType)
{
    public Type HandledType { get; } = handledType;
    public Type InterfaceService { get; } = interfaceService;
    public Type RelativeToGenericConcreteTypeInterfaceService { get; } = concreteType.IsGenericType && interfaceService.IsGenericType ? interfaceService.GetGenericTypeDefinition() : interfaceService;
    public Type ConcreteType { get; } = concreteType;
}
