using RMediator.Abstractions;

namespace RMediator.UnitTests.Handlers.Middlewares;

public class ASecondlyExecutedGenericTypedCommandMiddleware<TCommand>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}

public class ASecondlyExecutedGenericTypedCommandMiddleware<TCommand, TResult>(ExecutedMiddlewares middlewares) : IHandleMiddleware<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand request, NextMiddleware<TResult> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}
