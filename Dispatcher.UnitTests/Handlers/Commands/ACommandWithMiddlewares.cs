using Dispatcher.Commands;
using Dispatcher.Middlewares;

namespace Dispatcher.UnitTests.Handlers.Commands;

public record ACommandWithMiddlewares : ICommand;

public class ACommandWithMiddlewaresHandler : IHandleCommand<ACommandWithMiddlewares>
{
    public Task Handle(ACommandWithMiddlewares command, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class AStronglyTypedCommandMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<ACommandWithMiddlewares>
{
    public async Task Handle(ACommandWithMiddlewares request, NextMiddleware next, CancellationToken cancellationToken)
    {
        await next(request, cancellationToken);
        middlewares.MarkAsExecuted(GetType());
    }
}
