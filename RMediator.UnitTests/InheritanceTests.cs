using NFluent;
using RMediator.Abstractions;
using RMediator.UnitTests.Handlers.Middlewares;

namespace RMediator.UnitTests;

public class InheritanceTests : BaseTests
{
    [Fact]
    public async Task CommandMiddlewareOfBaseTypeAppliesToSubTypeInRegistrationOrder()
    {
        await CommandDispatcher.Dispatch(new SubCommand(), TestContext.Current.CancellationToken);

        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SubCommand>),
            typeof(ASecondlyExecutedGenericTypedCommandMiddleware<SubCommand>),
            typeof(BaseCommandMiddleware));
    }

    [Fact]
    public async Task CommandWithResultMiddlewareOfBaseTypeAppliesToSubType()
    {
        var result = await CommandDispatcher.Dispatch(new SubCommandWithResult(), TestContext.Current.CancellationToken);

        Check.That(result).Is("done");
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SubCommandWithResult, string>),
            typeof(ASecondlyExecutedGenericTypedCommandMiddleware<SubCommandWithResult, string>),
            typeof(BaseCommandWithResultMiddleware));
    }

    [Fact]
    public async Task QueryMiddlewareOfBaseTypeAppliesToSubType()
    {
        await QueryDispatcher.Dispatch(new SubQuery(), TestContext.Current.CancellationToken);

        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SubQuery, string>),
            typeof(ASecondlyExecutedGenericTypedQueryMiddleware<SubQuery, string>),
            typeof(BaseQueryMiddleware));
    }

    [Fact]
    public async Task DomainEventMiddlewareOfBaseTypeAppliesToSubType()
    {
        await DomainEventPublisher.Publish(new SubEvent(), TestContext.Current.CancellationToken);

        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SubEvent>),
            typeof(ASecondlyExecutedGenericTypedDomainEventMiddleware<SubEvent>),
            typeof(BaseEventMiddleware));
    }

    [Fact]
    public async Task MiddlewareOfBaseTypeIsNotExecutedTwiceForBaseTypeRequest()
    {
        await CommandDispatcher.Dispatch(new BaseCommand(), TestContext.Current.CancellationToken);

        Check.That(ExecutedMiddlewares.ExecutedTypes.Count(t => t == typeof(BaseCommandMiddleware))).Is(1);
    }
}

public record BaseCommand : ICommand;
public record SubCommand : BaseCommand;
public class BaseCommandHandler : IHandleCommand<BaseCommand>
{
    public Task Handle(BaseCommand command, CancellationToken cancellationToken) => Task.CompletedTask;
}

public record BaseCommandWithResult : ICommand<string>;
public record SubCommandWithResult : BaseCommandWithResult;
public class BaseCommandWithResultHandler : IHandleCommand<BaseCommandWithResult, string>
{
    public Task<string> Handle(BaseCommandWithResult command, CancellationToken cancellationToken) => Task.FromResult("done");
}

public record BaseQuery : IQuery<string>;
public record SubQuery : BaseQuery;
public class BaseQueryHandler : IHandleQuery<BaseQuery, string>
{
    public Task<string> Handle(BaseQuery query, CancellationToken cancellationToken) => Task.FromResult("done");
}

public record BaseEvent : IDomainEvent;
public record SubEvent : BaseEvent;
public class BaseEventListener : IListenToDomainEvent<BaseEvent>
{
    public Task Listen(BaseEvent @event, CancellationToken cancellationToken) => Task.CompletedTask;
}

public class BaseCommandMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<BaseCommand>
{
    public async Task Handle(BaseCommand request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}

public class BaseCommandWithResultMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<BaseCommandWithResult, string>
{
    public async Task<string> Handle(BaseCommandWithResult request, NextMiddleware<string> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}

public class BaseQueryMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<BaseQuery, string>
{
    public async Task<string> Handle(BaseQuery request, NextMiddleware<string> next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        return await next(request, cancellationToken);
    }
}

public class BaseEventMiddleware(ExecutedMiddlewares middlewares) : IHandleMiddleware<BaseEvent>
{
    public async Task Handle(BaseEvent request, NextMiddleware next, CancellationToken cancellationToken)
    {
        middlewares.MarkAsExecuted(GetType());
        await next(request, cancellationToken);
    }
}
