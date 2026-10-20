using RMediator.Abstractions;

namespace RMediator.DependencyInjection.Adapters;

internal class QueryDispatcher(HandlersRegistry registry, IServiceProvider services) : DispatcherBase(registry, services), IDispatchQuery
{
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return await SendRequest(query, cancellationToken);
    }
}
