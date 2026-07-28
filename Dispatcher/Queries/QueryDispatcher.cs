using Dispatcher.DependencyInjection;
using Dispatcher.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Queries;

public class QueryDispatcher(HandlersRegistry registry, IServiceProvider services) : IDispatchQuery
{
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default)
    {
        var handlers = registry.QueryHandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(query.GetType())).ToList();
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for query {query.GetType().Name}");
        }

        var middlewares = registry.MiddlewareHandlersDefinition.Where(t => t.HandledType.IsAssignableFrom(query.GetType())).ToList();
        var serviceType = handlers.First().HandledInterface;
        var implem = middlewares.SelectMany(h => services
            .GetServices(h.HandledInterface))
            .Where(h => h is not null)
            .DistinctBy(h => h!.GetType())
            .OfType<IHandleMiddleware<TResponse>>()
            .ToList();

        if (implem.Count == 0)
        {
            return await ((IHandleRequest<TResponse>)services.GetRequiredService(serviceType)).Handle(query, cancellationToken ?? CancellationToken.None);
        }

        return await implem.First().Handle(query, GetNextMiddleware(implem, 0, serviceType), cancellationToken ?? CancellationToken.None);
    }

    private NextMiddleware<TResponse> GetNextMiddleware<TResponse>(IReadOnlyCollection<IHandleMiddleware<TResponse>> middlewares, int index, Type serviceType)
    {
        if (index == middlewares.Count - 1)
        {
            return async (query, token) => await ((IHandleRequest<TResponse>)services.GetRequiredService(serviceType)).Handle((IQuery<TResponse>)query, token);
        }
        return async (query, token) => await middlewares.ElementAt(index + 1).Handle(query, GetNextMiddleware(middlewares, index + 1, serviceType), token);
    }
}