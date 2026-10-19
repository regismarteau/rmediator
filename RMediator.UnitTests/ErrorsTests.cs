using NFluent;
using RMediator.Abstractions;

namespace RMediator.UnitTests;

public class ErrorsTests : BaseTests
{
    [Fact]
    public async Task PublishWaitsForListenersCompletion()
    {
        await DomainEventPublisher.Publish(new SlowEvent());

        Check.That(GetService<EventProbe>().SlowListenerCompleted).IsTrue();
    }

    [Fact]
    public async Task PublishPropagatesListenerFailures()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => DomainEventPublisher.Publish(new FailingEvent()));

        Check.That(exception.Message).Is("listener failure");
    }

    [Fact]
    public async Task CommandHandlerExceptionIsNotWrappedInTargetInvocationException()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CommandDispatcher.Dispatch(new FailingCommand()));

        Check.That(exception.Message).Is("command failure");
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
