using RMediator.Abstractions;

namespace RMediator.UnitTests.Handlers.Commands;

public record SomeCommandWithResult : ICommand<SomeCommandResult>;

public record SomeCommandResult(string Value);

public partial class SomeCommandHandler : IHandleCommand<SomeCommandWithResult, SomeCommandResult>
{
    public Task<SomeCommandResult> Handle(SomeCommandWithResult query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeCommandResult("A command result!"));
    }
}