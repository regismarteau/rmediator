using Dispatcher.UnitTests.Handlers.Commands;
using Dispatcher.UnitTests.Handlers.Middlewares;
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

    [Theory]
    [InlineData("someone")]
    [InlineData("another one")]
    public async Task ShouldExecuteCommand(string requestedBy)
    {
        await CommandDispatcher.Dispatch(new SomeCommand(requestedBy));
        Check.That(GetService<CommandTriggered>().TriggeredBy).Is(requestedBy);
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
        Check.That(GetService<CommandTriggered>().HasBeenTriggered).IsFalse();
    }

    [Fact]
    public async Task CommandCorrespondingMiddlewaresAreExecutedInOrder()
    {
        await this.CommandDispatcher.Dispatch(new ACommandWithMiddlewares());
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGenericTypedCommandMiddleware<ACommandWithMiddlewares>),
            typeof(ASecondlyExecutedStronglyTypedCommandMiddleware));
    }

    [Fact]
    public async Task CommandWithResultCorrespondingMiddlewaresAreExecutedInOrder()
    {
        await this.CommandDispatcher.Dispatch(new ACommandWithResultWithMiddlewares());
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGenericTypedCommandWithResultMiddleware<ACommandWithResultWithMiddlewares, SomeCommandResult>),
            typeof(ASecondlyExecutedStronglyTypedCommandWithResultMiddleware));
    }
}
