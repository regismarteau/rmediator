using Dispatcher.DependencyInjection;
using Dispatcher.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Requests;

public class BaseRequestDispatcher(HandlersRegistry registry, IServiceProvider services)
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
        var middlewareServices = GetMiddlewareFor(request);

        if (middlewareServices.Count == 0)
        {
            await ((IHandleRequest)services.GetRequiredService(handler.HandledInterface)).Handle(request, cancellationToken);
            return;
        }

        await middlewareServices.First().Handle(request, GetNextMiddleware(middlewareServices, handler.HandledInterface, 0), cancellationToken);
    }

    protected List<IInternalHandleMiddleware> GetMiddlewareFor(IRequest request)
    {
        return [.. registry.MiddlewareHandlersDefinition
            .Where(t => t.HandledType.IsAssignableFrom(request.GetType()))
            .SelectMany(h => services.GetServices(h.HandledInterface.GetGenericTypeDefinition().MakeGenericType(request.GetType())))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IInternalHandleMiddleware>()];
    }

    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, HandlerDefinition handler, CancellationToken cancellationToken)
    {
        var middlewareServices = registry.MiddlewareHandlersDefinition
            .Where(t => t.HandledType.IsAssignableFrom(request.GetType()))
            .SelectMany(h => services.GetServices(h.HandledInterface.GetGenericTypeDefinition().MakeGenericType(request.GetType(), typeof(TResponse))))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IInternalHandleMiddleware<TResponse>>()
            .ToList();

        if (middlewareServices.Count == 0)
        {
            return await ((IHandleRequest<TResponse>)services.GetRequiredService(handler.HandledInterface)).Handle(request, cancellationToken);
        }

        return await middlewareServices.First().Handle(request, GetNextMiddleware(middlewareServices, handler.HandledInterface, 0), cancellationToken);
    }

    protected IReadOnlyCollection<HandlerDefinition> GetHandlersFor(IRequest request)
    {
        return registry.HandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(request.GetType())).ToList();
    }

    private NextMiddleware<TResponse> GetNextMiddleware<TResponse>(IReadOnlyCollection<IInternalHandleMiddleware<TResponse>> middlewares, Type handlerServiceType, int index)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest<TResponse>)services.GetRequiredService(handlerServiceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, handlerServiceType, index + 1), token);
    }

    private NextMiddleware GetNextMiddleware(IReadOnlyCollection<IInternalHandleMiddleware> middlewares, Type handlerServiceType, int index)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest)services.GetRequiredService(handlerServiceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, handlerServiceType, index + 1), token);
    }
}