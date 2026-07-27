namespace Dispatcher.UnitTests.Handlers.Commands;

public class CommandTriggered
{
    public bool Triggered { get; private set; }

    public void Trigger()
    {
        this.Triggered = true;
    }
}