using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers.Commands;


public record SomeCommand(string RequestedBy) : ICommand;
public record SomeSubCommand(string RequestedBy) : SomeCommand(RequestedBy);

public partial class SomeCommandHandler(CommandTriggered commandTrigger) : IHandleCommand<SomeCommand>
{
    public Task Handle(SomeCommand command, CancellationToken cancellationToken)
    {
        commandTrigger.TriggerBy(command.RequestedBy);
        commandTrigger.Complete();
        return Task.CompletedTask;
    }
}