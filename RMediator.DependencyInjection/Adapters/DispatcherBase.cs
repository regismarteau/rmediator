using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal abstract class DispatcherBase(HandlersRegistry registry, IServiceProvider services)
{
    protected IServiceProvider Services { get; } = services;

    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var handler = GetSingleHandler(request);
        var middlewareServices = GetMiddlewares(request.GetType(), typeof(TResponse))
            .OfType<IAbstractMiddlewareHandler<TResponse>>()
            .ToList();

        return await middlewareServices
            .Wrap(((IHandleRequestWithResponse<TResponse>)Services.GetRequiredService(handler.InterfaceService)).Handle)
            .Invoke(request, cancellationToken);
    }

    protected List<IAbstractMiddlewareHandler> GetMiddlewareServicesFor(IRequest request)
    {
        return [.. GetMiddlewares(request.GetType(), null).OfType<IAbstractMiddlewareHandler>()];
    }

    protected HandlerDefinition GetSingleHandler(IRequest request)
    {
        var handlers = GetHandlersFor(request);
        return handlers.Count switch
        {
            1 => handlers.First(),
            0 => throw new InvalidOperationException($"No handler is registered for {request.GetType().Name}"),
            _ => throw new InvalidOperationException($"Several handlers are registered for {request.GetType().Name}: {string.Join(", ", handlers.Select(h => h.ConcreteType.Name))}")
        };
    }

    protected IReadOnlyCollection<HandlerDefinition> GetHandlersFor(IRequest request)
    {
        return registry.GetHandlersFor(request.GetType());
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
        return registry.GetBaseTypeMiddlewaresFor(requestType, responseType)
            .SelectMany(d => GetServicesOf(d.InterfaceService).Where(m => m.GetType() == d.ConcreteType));
    }

    private IEnumerable<object> GetServicesOf(Type serviceType)
    {
        return Services.GetServices(serviceType).OfType<object>();
    }
}
