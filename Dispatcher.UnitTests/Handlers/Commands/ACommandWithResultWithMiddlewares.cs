using Dispatcher.Commands;
using Dispatcher.Middlewares;

namespace Dispatcher.UnitTests.Handlers.Commands;

public record ACommandWithResultWithMiddlewares : ICommand<SomeCommandResult>;

public class ACommandWithResultWithMiddlewaresHandler : IHandleCommand<ACommandWithResultWithMiddlewares, SomeCommandResult>
{
    public Task<SomeCommandResult> Handle(ACommandWithResultWithMiddlewares command, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeCommandResult("Executed!"));
    }
}

public class AStronglyTypedCommandWithResultMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<ACommandWithResultWithMiddlewares, SomeCommandResult>
{
    public async Task<SomeCommandResult> Handle(ACommandWithResultWithMiddlewares request, NextMiddleware<SomeCommandResult> next, CancellationToken cancellationToken)
    {
        var result = await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
        return result;
    }
}
