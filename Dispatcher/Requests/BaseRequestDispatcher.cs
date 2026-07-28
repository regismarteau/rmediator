using Dispatcher.DependencyInjection;
using Dispatcher.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Requests;

public class BaseRequestDispatcher(HandlersRegistry registry, IServiceProvider services)
{
    protected HandlerDefinition GetSingleHandler(IRequest request)
    {
        var handlers = registry.AllDefinitions.Where(t => t.HandledType.IsAssignableFrom(request.GetType())).ToList();
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for {request.GetType().Name}");
        }

        return handlers[0];
    }

    protected async Task SendRequest(IRequest request, HandlerDefinition handler, CancellationToken cancellationToken)
    {
        var middlewareServices = registry.MiddlewareHandlersDefinition
            .Where(t => t.HandledType.IsAssignableFrom(request.GetType()))
            .SelectMany(h => services.GetServices(h.HandledInterface.GetGenericTypeDefinition().MakeGenericType(request.GetType())))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IInternalHandleMiddleware>()
            .ToList();

        if (middlewareServices.Count == 0)
        {
            await ((IHandleRequest)services.GetRequiredService(handler.HandledInterface)).Handle(request, cancellationToken);
            return;
        }

        await middlewareServices.First().Handle(request, GetNextMiddleware(middlewareServices, 0, handler.HandledInterface), cancellationToken);
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

        return await middlewareServices.First().Handle(request, GetNextMiddleware(middlewareServices, 0, handler.HandledInterface), cancellationToken);
    }

    private NextMiddleware<TResponse> GetNextMiddleware<TResponse>(IReadOnlyCollection<IInternalHandleMiddleware<TResponse>> middlewares, int index, Type serviceType)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest<TResponse>)services.GetRequiredService(serviceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, index + 1, serviceType), token);
    }

    private NextMiddleware GetNextMiddleware(IReadOnlyCollection<IInternalHandleMiddleware> middlewares, int index, Type serviceType)
    {
        if (index == middlewares.Count - 1)
        {
            return async (request, token) => await ((IHandleRequest)services.GetRequiredService(serviceType)).Handle(request, token);
        }
        return async (request, token) => await middlewares.ElementAt(index + 1).Handle(request, GetNextMiddleware(middlewares, index + 1, serviceType), token);
    }
}