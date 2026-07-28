using Dispatcher.DependencyInjection;
using Dispatcher.Requests;

namespace Dispatcher.Queries;

public class QueryDispatcher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IDispatchQuery
{
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default)
    {
        return await SendRequest(query, GetSingleHandler(query), cancellationToken ?? CancellationToken.None);
    }
}