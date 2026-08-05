using RMediator.Abstractions;
using RMediator.UnitTests.Handlers.Middlewares;

namespace RMediator.UnitTests.Handlers.Commands;

public record ACommandWithResultWithMiddlewares : ICommand<SomeCommandResult>;

public class ACommandWithResultWithMiddlewaresHandler : IHandleCommand<ACommandWithResultWithMiddlewares, SomeCommandResult>
{
    public Task<SomeCommandResult> Handle(ACommandWithResultWithMiddlewares command, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeCommandResult("Executed!"));
    }
}

public class AThirdlyExecutedStronglyTypedCommandWithResultMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<ACommandWithResultWithMiddlewares, SomeCommandResult>
{
    public async Task<SomeCommandResult> Handle(ACommandWithResultWithMiddlewares request, NextMiddleware<SomeCommandResult> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}
