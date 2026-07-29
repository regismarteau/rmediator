namespace Dispatcher.UnitTests.Handlers.Commands;

public class CommandTriggered
{
    public string? TriggeredBy { get; private set; }

    public bool Completed { get; private set; }

    public void TriggerBy(string requester)
    {
        TriggeredBy = requester;
    }

    public void Complete()
    {
        Completed = true;
    }
}