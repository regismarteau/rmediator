using Dispatcher.Commands;

namespace Dispatcher.UnitTests.Handlers.Commands;


public record SomeCommand : ICommand;

public partial class SomeCommandHandler(CommandTriggered commandTrigger) : IHandleCommand<SomeCommand>
{
    public Task Handle(SomeCommand command, CancellationToken cancellationToken)
    {
        commandTrigger.Trigger();
        return Task.CompletedTask;
    }
}