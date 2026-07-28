using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers.Commands;


public record SomeCommand(string RequestedBy) : ICommand;

public partial class SomeCommandHandler(CommandTriggered commandTrigger) : IHandleCommand<SomeCommand>
{
    public Task Handle(SomeCommand command, CancellationToken cancellationToken)
    {
        commandTrigger.TriggerBy(command.RequestedBy);
        return Task.CompletedTask;
    }
}