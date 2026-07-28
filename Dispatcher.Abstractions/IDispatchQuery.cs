namespace Dispatcher.Abstractions;

public interface IDispatchQuery
{
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default);
}

public interface IQuery<out TResponse> : IRequest<TResponse>;

public interface IHandleQuery<in TQuery, TResponse> : IHandleRequest<TResponse> where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);

    async Task<TResponse> IHandleRequest<TResponse>.Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        return await Handle((TQuery)request, cancellationToken);
    }
}