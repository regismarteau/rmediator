using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers.Commands;

public record ACancellableCommand(string RequestedBy) : ICommand;

public class ACancellableCommandHandler(CommandTriggered commandTrigger) : IHandleCommand<ACancellableCommand>
{
    public async Task Handle(ACancellableCommand command, CancellationToken cancellationToken)
    {

        commandTrigger.TriggerBy(command.RequestedBy);
        try
        {
            var semaphore = new SemaphoreSlim(0);
            await semaphore.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        commandTrigger.Complete();
    }
}