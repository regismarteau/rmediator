using NFluent;
using RMediator.Abstractions;

namespace RMediator.UnitTests;

public class ErrorsTests : BaseTests
{
    [Fact]
    public async Task PublishWaitsForListenersCompletion()
    {
        await DomainEventPublisher.Publish(new SlowEvent(), TestContext.Current.CancellationToken);

        Check.That(GetService<EventProbe>().SlowListenerCompleted).IsTrue();
    }

    [Fact]
    public async Task PublishPropagatesListenerFailures()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => DomainEventPublisher.Publish(new FailingEvent(), TestContext.Current.CancellationToken));

        Check.That(exception.Message).Is("listener failure");
    }

    [Fact]
    public async Task CommandHandlerExceptionIsNotWrappedInTargetInvocationException()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CommandDispatcher.Dispatch(new FailingCommand(), TestContext.Current.CancellationToken));

        Check.That(exception.Message).Is("command failure");
    }

    [Fact]
    public async Task DispatchingARequestWithoutHandlerExplainsThereIsNone()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CommandDispatcher.Dispatch(new UnhandledCommand(), TestContext.Current.CancellationToken));

        Check.That(exception.Message).Is("No handler is registered for UnhandledCommand");
    }

    [Fact]
    public async Task DispatchingARequestWithSeveralHandlersListsThem()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CommandDispatcher.Dispatch(new DuplicatedCommand(), TestContext.Current.CancellationToken));

        Check.That(exception.Message).Is("Several handlers are registered for DuplicatedCommand: FirstDuplicatedCommandHandler, SecondDuplicatedCommandHandler");
    }

    [Fact]
    public async Task RepeatedDispatchesExecuteTheSameMiddlewaresEachTime()
    {
        await CommandDispatcher.Dispatch(new SubCommand(), TestContext.Current.CancellationToken);
        var firstRun = ExecutedMiddlewares.ExecutedTypes.ToList();
        ExecutedMiddlewares.ExecutedTypes.Clear();

        await CommandDispatcher.Dispatch(new SubCommand(), TestContext.Current.CancellationToken);

        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(firstRun);
    }
}

public record SlowEvent : IDomainEvent;
public class SlowEventListener(EventProbe probe) : IListenToDomainEvent<SlowEvent>
{
    public async Task Listen(SlowEvent @event, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        probe.SlowListenerCompleted = true;
    }
}

public record FailingEvent : IDomainEvent;
public class FailingEventListener : IListenToDomainEvent<FailingEvent>
{
    public async Task Listen(FailingEvent @event, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new InvalidOperationException("listener failure");
    }
}

public record FailingCommand : ICommand;
public class FailingCommandHandler : IHandleCommand<FailingCommand>
{
    public Task Handle(FailingCommand command, CancellationToken cancellationToken) => throw new InvalidOperationException("command failure");
}

public record UnhandledCommand : ICommand;

public record DuplicatedCommand : ICommand;
public class FirstDuplicatedCommandHandler : IHandleCommand<DuplicatedCommand>
{
    public Task Handle(DuplicatedCommand command, CancellationToken cancellationToken) => Task.CompletedTask;
}
public class SecondDuplicatedCommandHandler : IHandleCommand<DuplicatedCommand>
{
    public Task Handle(DuplicatedCommand command, CancellationToken cancellationToken) => Task.CompletedTask;
}
