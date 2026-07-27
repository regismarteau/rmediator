using Dispatcher.DomainEvents;

namespace Dispatcher.UnitTests.Handlers.DomainEvents;

public class DomainEventListened
{
    public string? RaisedBy { get; private set; }

    public IList<Type> ListenedBy = [];

    public bool HasBeenRaised => !string.IsNullOrEmpty(RaisedBy);

    public void RaiseBy(string requester)
    {
        this.RaisedBy = requester;
    }

    public void Listen(Type listener)
    {
        this.ListenedBy.Add(listener);
    }
}