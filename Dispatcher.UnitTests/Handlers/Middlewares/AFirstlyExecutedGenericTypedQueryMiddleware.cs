using Dispatcher.Middlewares;
using Dispatcher.Queries;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class AFirstlyExecutedGenericTypedQueryMiddleware<TQuery, TResponse>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}
