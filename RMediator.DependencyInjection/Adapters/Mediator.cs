using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal class Mediator(HandlersRegistry registry, IServiceProvider services)
{
    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var handler = GetSingleHandler(request);
        var middlewareServices = GetMiddlewares(request.GetType(), typeof(TResponse))
            .OfType<IAbstractMiddlewareHandler<TResponse>>()
            .ToList();

        return await middlewareServices
            .Wrap(((IHandleRequestWithResponse<TResponse>)services.GetRequiredService(handler.InterfaceService)).Handle)
            .Invoke(request, cancellationToken);
    }

    protected List<IAbstractMiddlewareHandler> GetMiddlewareServicesFor(IRequest request)
    {
        return [.. GetMiddlewares(request.GetType(), null).OfType<IAbstractMiddlewareHandler>()];
    }

    private List<object> GetMiddlewares(Type requestType, Type? responseType)
    {
        return [.. GetExactTypeMiddlewares(requestType, responseType)
            .Concat(GetBaseTypeMiddlewares(requestType, responseType))
            .OrderBy(m => registry.GetMiddlewareOrder(m.GetType()))];
    }

    private IEnumerable<object> GetExactTypeMiddlewares(Type requestType, Type? responseType)
    {
        var serviceType = responseType is null
            ? typeof(IHandleMiddleware<>).MakeGenericType(requestType)
            : typeof(IHandleMiddleware<,>).MakeGenericType(requestType, responseType);

        return GetServicesOf(serviceType);
    }

    private IEnumerable<object> GetBaseTypeMiddlewares(Type requestType, Type? responseType)
    {
        return registry.MiddlewareHandlersDefinition
            .Where(d => !d.ConcreteType.IsGenericTypeDefinition
                && d.HandledType != requestType
                && d.HandledType.IsAssignableFrom(requestType)
                && IsMiddlewareServiceFor(d.InterfaceService, responseType))
            .DistinctBy(d => d.InterfaceService)
            .SelectMany(d => GetServicesOf(d.InterfaceService).Where(m => m.GetType() == d.ConcreteType));
    }

    private IEnumerable<object> GetServicesOf(Type serviceType)
    {
        return services.GetServices(serviceType).OfType<object>();
    }

    private static bool IsMiddlewareServiceFor(Type middlewareService, Type? responseType)
    {
        return responseType is null
            ? middlewareService.GetGenericTypeDefinition() == typeof(IHandleMiddleware<>)
            : middlewareService.GetGenericTypeDefinition() == typeof(IHandleMiddleware<,>)
                && middlewareService.GetGenericArguments()[1] == responseType;
    }

    protected HandlerDefinition GetSingleHandler(IRequest request)
    {
        var handlers = GetHandlersFor(request);
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for {request.GetType().Name}");
        }

        return handlers.First();
    }

    protected IReadOnlyCollection<HandlerDefinition> GetHandlersFor(IRequest request)
    {
        return [.. registry.HandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(request.GetType()))];
    }
}