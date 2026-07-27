namespace Dispatcher.UnitTests.Handlers.DomainEvents;

public class DomainEventListened
{
    public string? RaisedBy { get; private set; }

    public bool HasBeenRaised => !string.IsNullOrEmpty(RaisedBy);

    public void RaiseBy(string requester)
    {
        this.RaisedBy = requester;
    }
}