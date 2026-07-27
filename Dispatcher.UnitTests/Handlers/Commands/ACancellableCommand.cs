using Dispatcher.Commands;

namespace Dispatcher.UnitTests.Handlers.Commands;

public record ACancellableCommand : ICommand;

public class ACancellableCommandHandler(CommandTriggered commandTrigger) : IHandleCommand<ACancellableCommand>
{
    public async Task Handle(ACancellableCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var semaphore = new SemaphoreSlim(0);
            await semaphore.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        commandTrigger.Trigger();
    }
}