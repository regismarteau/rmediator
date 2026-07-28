namespace Dispatcher.UnitTests.Handlers.Commands;

public class CommandTriggered
{
    public string? TriggeredBy { get; private set; }

    public bool HasBeenTriggered => !string.IsNullOrEmpty(TriggeredBy);

    public void TriggerBy(string requester)
    {
        TriggeredBy = requester;
    }
}