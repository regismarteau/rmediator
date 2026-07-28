namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class ExecutedMiddlewares
{
    public IList<Type> ExecutedTypes { get; } = [];

    public void MarkAsExecuted(Type type)
    {
        ExecutedTypes.Add(type);
    }
}