using Dispatcher.Middlewares;
using Dispatcher.Queries;

namespace Dispatcher.UnitTests.Handlers.Queries;

public record AQueryWithMiddlewares : IQuery<SomeQueryResponse>;

public class AQueryWithMiddlewaresHandler : IHandleQuery<AQueryWithMiddlewares, SomeQueryResponse>
{
    public Task<SomeQueryResponse> Handle(AQueryWithMiddlewares query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeQueryResponse($"Queried!"));
    }
}

public class AStronglyTypedQueryMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<AQueryWithMiddlewares, SomeQueryResponse>
{
    public async Task<SomeQueryResponse> Handle(AQueryWithMiddlewares request, NextMiddleware<SomeQueryResponse> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}
