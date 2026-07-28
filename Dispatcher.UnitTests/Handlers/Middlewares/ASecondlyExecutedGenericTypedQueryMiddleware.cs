using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class ASecondlyExecutedGenericTypedQueryMiddleware<TQuery, TResponse>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}
