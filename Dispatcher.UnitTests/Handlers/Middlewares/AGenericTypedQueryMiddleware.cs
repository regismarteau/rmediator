using Dispatcher.Middlewares;
using Dispatcher.Queries;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class AGenericTypedQueryMiddleware<TQuery, TResponse>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}
