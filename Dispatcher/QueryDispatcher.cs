using Dispatcher.Abstractions;
using Dispatcher.DependencyInjection;

namespace Dispatcher;

internal class QueryDispatcher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IDispatchQuery
{
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default)
    {
        return await SendRequest(query, GetSingleHandler(query), cancellationToken ?? CancellationToken.None);
    }
}