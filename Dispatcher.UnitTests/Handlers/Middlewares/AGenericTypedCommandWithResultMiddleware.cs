using Dispatcher.Commands;
using Dispatcher.Middlewares;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class AGenericTypedCommandMiddleware<TCommand>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand request, NextMiddleware next, CancellationToken cancellationToken)
    {
        await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
    }
}

public class AGenericTypedCommandWithResultMiddleware<TCommand, TResult>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand request, NextMiddleware<TResult> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}