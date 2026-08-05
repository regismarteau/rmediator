using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Extensions;

namespace RMediator.DependencyInjection.Adapters;

internal class Mediator(HandlersRegistry registry, IServiceProvider services)
{
    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var handler = GetSingleHandler(request);
        var middlewareServices = services.GetServices(typeof(IHandleMiddleware<,>).MakeGenericType(request.GetType(), typeof(TResponse)))
            .Where(h => h is not null)
            .OfType<IAbstractMiddlewareHandler<TResponse>>()
            .ToList();

        return await middlewareServices
            .Wrap(((IHandleRequestWithResponse<TResponse>)services.GetRequiredService(handler.InterfaceService)).Handle)
            .Invoke(request, cancellationToken);
    }

    protected List<IAbstractMiddlewareHandler> GetMiddlewareServicesFor(IRequest request)
    {
        return [.. services.GetServices(typeof(IHandleMiddleware<>).MakeGenericType(request.GetType()))
            .Where(h => h is not null)
            .OfType<IAbstractMiddlewareHandler>()];
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