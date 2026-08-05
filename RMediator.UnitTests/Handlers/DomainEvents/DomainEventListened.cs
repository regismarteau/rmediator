namespace RMediator.UnitTests.Handlers.DomainEvents;

public class DomainEventListened
{
    public string? RaisedBy { get; private set; }

    public IList<Type> ListenedBy = [];

    public bool HasBeenRaised => !string.IsNullOrEmpty(RaisedBy);

    public void RaiseBy(string requester)
    {
        RaisedBy = requester;
    }

    public void Listen(Type listener)
    {
        ListenedBy.Add(listener);
    }
}