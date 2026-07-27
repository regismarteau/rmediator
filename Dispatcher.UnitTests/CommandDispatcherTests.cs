using Dispatcher.UnitTests.Handlers.Commands;
using Microsoft.Extensions.DependencyInjection;
using NFluent;

namespace Dispatcher.UnitTests;

public class CommandDispatcherTests : BaseTests
{
    public override IServiceCollection Initialize(IServiceCollection services)
    {
        return base.Initialize(services)
            .AddSingleton<CommandTriggered>();
    }

    [Fact]
    public async Task ShouldExecuteCommand()
    {
        await CommandDispatcher.Dispatch(new SomeCommand());
        Check.That(GetService<CommandTriggered>().Triggered).IsTrue();
    }

    [Fact]
    public async Task ShouldReturnExecutionResult()
    {
        var result = await CommandDispatcher.Dispatch(new SomeCommandWithResult());
        Check.That(result.Value).Is("A command result!");
    }

    [Fact]
    public async Task CouldBeCanceled()
    {
        var token = new CancellationTokenSource();
        await Task.WhenAll(
            Task.Factory.StartNew(async () => await CommandDispatcher.Dispatch(new ACancellableCommand(), token.Token), TestContext.Current.CancellationToken),
            Task.Factory.StartNew(token.Cancel, TestContext.Current.CancellationToken));
        Check.That(GetService<CommandTriggered>().Triggered).IsFalse();
    }
}
