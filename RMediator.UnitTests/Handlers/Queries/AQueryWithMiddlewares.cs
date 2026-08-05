using RMediator.Abstractions;
using RMediator.UnitTests.Handlers.Middlewares;

namespace RMediator.UnitTests.Handlers.Queries;

public record AQueryWithMiddlewares : IQuery<SomeQueryResponse>;

public class AQueryWithMiddlewaresHandler : IHandleQuery<AQueryWithMiddlewares, SomeQueryResponse>
{
    public Task<SomeQueryResponse> Handle(AQueryWithMiddlewares query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeQueryResponse($"Queried!"));
    }
}

public class AThirdlyExecutedStronglyTypedQueryMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<AQueryWithMiddlewares, SomeQueryResponse>
{
    public async Task<SomeQueryResponse> Handle(AQueryWithMiddlewares request, NextMiddleware<SomeQueryResponse> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}
