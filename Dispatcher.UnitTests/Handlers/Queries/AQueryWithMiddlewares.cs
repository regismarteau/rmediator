using Dispatcher.Middlewares;
using Dispatcher.Queries;
using Dispatcher.Requests;

namespace Dispatcher.UnitTests.Handlers.Queries;

public record AQueryWithMiddlewares : IQuery<SomeQueryResponse>;

public class AQueryWithMiddlewaresHandler : IHandleQuery<AQueryWithMiddlewares, SomeQueryResponse>
{
    public Task<SomeQueryResponse> Handle(AQueryWithMiddlewares query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeQueryResponse($"Queried!"));
    }
}

public class AFirstStronglyTypedQueryMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<AQueryWithMiddlewares, SomeQueryResponse>
{
    public async Task<SomeQueryResponse> Handle(AQueryWithMiddlewares request, NextMiddleware<SomeQueryResponse> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}

public class ASecondGenericTypedQueryMiddleware<TQuery, TResponse>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TQuery, TResponse> where TQuery : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TQuery request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}

public class ExecutedMiddlewares
{
    public IList<Type> ExecutedTypes { get; } = [];

    public void MarkAsExecuted(Type type)
    {
        ExecutedTypes.Add(type);
    }
}