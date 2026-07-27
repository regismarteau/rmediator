using Dispatcher.DependencyInjection;

namespace Dispatcher.Queries;

public interface IDispatchQuery
{
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken? cancellationToken = default);
}

public interface IQuery<TResponse>;

public interface IHandleQuery<TQuery, TResponse> : IHandleRequest where TQuery : IQuery<TResponse> where TResponse : notnull
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);

    async Task<object> IHandleRequest.Handle(object request, CancellationToken cancellationToken)
    {
        return await this.Handle((TQuery)request, cancellationToken);
    }
}