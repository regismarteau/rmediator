using Dispatcher.DependencyInjection;
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

        return (TResponse)await ((IHandleRequest)services.GetRequiredService(handlers.First().HandledInterface)).Handle(query, cancellationToken ?? CancellationToken.None);
    }
}