using Dispatcher.DependencyInjection;
using Dispatcher.Extensions;
using Dispatcher.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Requests;

internal class BaseRequestDispatcher(HandlersRegistry registry, IServiceProvider services)
{
    protected HandlerDefinition GetSingleHandler(IRequest request)
    {
        var handlers = GetHandlersFor(request);
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for {request.GetType().Name}");
        }

        return handlers.First();
    }

    protected async Task SendRequest(IRequest request, HandlerDefinition handler, CancellationToken cancellationToken)
    {
        var middlewareServices = GetMiddlewareServicesFor(request);

        await middlewareServices
            .Wrap(((IHandleRequest)services.GetRequiredService(handler.HandledInterface)).Handle)
            .Invoke(request, cancellationToken);
    }

    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, HandlerDefinition handler, CancellationToken cancellationToken)
    {
        var middlewareServices = registry.MiddlewareHandlersDefinition
            .Where(t => t.HandledType.IsAssignableFrom(request.GetType()))
            .SelectMany(h => services.GetServices(h.HandledInterface.GetGenericTypeDefinition().MakeGenericType(request.GetType(), typeof(TResponse))))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IAbstractMiddlewareHandler<TResponse>>()
            .ToList();

        return await middlewareServices
            .Wrap(((IHandleRequest<TResponse>)services.GetRequiredService(handler.HandledInterface)).Handle)
            .Invoke(request, cancellationToken);
    }

    protected List<IAbstractMiddlewareHandler> GetMiddlewareServicesFor(IRequest request)
    {
        return [.. registry.MiddlewareHandlersDefinition
            .Where(t => t.HandledType.IsAssignableFrom(request.GetType()))
            .SelectMany(h => services.GetServices(h.HandledInterface.GetGenericTypeDefinition().MakeGenericType(request.GetType())))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IAbstractMiddlewareHandler>()];
    }

    protected IReadOnlyCollection<HandlerDefinition> GetHandlersFor(IRequest request)
    {
        return registry.HandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(request.GetType())).ToList();
    }

    private NextMiddleware<TResponse> GetNextMiddleware<TResponse>(IReadOnlyCollection<IAbstractMiddlewareHandler<TResponse>> middlewares, Type handlerServiceType, int index)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest<TResponse>)services.GetRequiredService(handlerServiceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, handlerServiceType, index + 1), token);
    }

    private NextMiddleware GetNextMiddleware(IReadOnlyCollection<IAbstractMiddlewareHandler> middlewares, Type handlerServiceType, int index)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest)services.GetRequiredService(handlerServiceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, handlerServiceType, index + 1), token);
    }
}