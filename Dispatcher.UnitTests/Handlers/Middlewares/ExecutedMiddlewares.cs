using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers.Middlewares;

public class ExecutedMiddlewares
{
    public IList<Type> ExecutedTypes { get; } = [];

    public void MarkAsExecuted(Type type)
    {
        ExecutedTypes.Add(type);
    }
}

public class HandlersCalled
{
    private readonly List<HandlerCalled> handlers = [];

    public CalledHandlerBuilder MarkAsCalled(object handler)
    {
        return new(this, handler);
    }

    public class CalledHandlerBuilder(HandlersCalled handlers, object handler)
    {
        public void WithRequest(IRequest request)
        {
            handlers.handlers.Add(new HandlerCalled(handler, request));
        }
    }

    public record HandlerCalled(object Handler, IRequest Request);
}