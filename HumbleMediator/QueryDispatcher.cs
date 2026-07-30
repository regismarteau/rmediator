using HumbleMediator.Abstractions;
using HumbleMediator.DependencyInjection;

namespace HumbleMediator;

internal class QueryDispatcher(HandlersRegistry registry, IServiceProvider services) : Mediator(registry, services), IDispatchQuery
{
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default)
    {
        return await SendRequest(query, cancellationToken ?? CancellationToken.None);
    }
}