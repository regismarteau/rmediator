using Dispatcher.Middlewares;
using Dispatcher.Requests;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

internal class AFirstlyExecutedGeneralMiddleware<TRequest, TResponse>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}

internal class AFirstlyExecutedGeneralMiddleware<TRequest>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TRequest> where TRequest : IRequest
{
    public async Task Handle(TRequest request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}
