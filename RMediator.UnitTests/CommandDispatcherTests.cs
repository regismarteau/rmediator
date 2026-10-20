using Microsoft.Extensions.DependencyInjection;
using NFluent;
using RMediator.UnitTests.Handlers.Commands;
using RMediator.UnitTests.Handlers.Middlewares;

namespace RMediator.UnitTests;

public class CommandDispatcherTests : BaseTests
{
    public override IServiceCollection Initialize(IServiceCollection services)
    {
        return base.Initialize(services)
            .AddSingleton<CommandTriggered>();
    }

    [Theory]
    [InlineData("someone")]
    [InlineData("another guy")]
    public async Task ShouldExecuteCommand(string requestedBy)
    {
        await CommandDispatcher.Dispatch(new SomeCommand(requestedBy), TestContext.Current.CancellationToken);
        Check.That(GetService<CommandTriggered>().TriggeredBy).Is(requestedBy);
    }

    [Fact]
    public async Task ASubCommandCanTriggerAnHandler()
    {
        await CommandDispatcher.Dispatch(new SomeSubCommand("me"), TestContext.Current.CancellationToken);
        Check.That(GetService<CommandTriggered>().TriggeredBy).Is("me");
    }

    [Fact]
    public async Task ShouldReturnExecutionResult()
    {
        var result = await CommandDispatcher.Dispatch(new SomeCommandWithResult(), TestContext.Current.CancellationToken);
        Check.That(result.Value).Is("A command result!");
    }

    [Fact]
    public async Task CouldBeCanceled()
    {
        var token = new CancellationTokenSource();
        await Task.WhenAll(
            Task.Run(async () => await CommandDispatcher.Dispatch(new ACancellableCommand("me"), token.Token), TestContext.Current.CancellationToken),
            Task.Factory.StartNew(token.Cancel, TestContext.Current.CancellationToken));
        Check.That(GetService<CommandTriggered>().TriggeredBy).Is("me");
        Check.That(GetService<CommandTriggered>().Completed).IsFalse();
    }

    [Fact]
    public async Task CommandCorrespondingMiddlewaresAreExecutedInOrder()
    {
        await CommandDispatcher.Dispatch(new ACommandWithMiddlewares(), TestContext.Current.CancellationToken);
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<ACommandWithMiddlewares>),
            typeof(ASecondlyExecutedGenericTypedCommandMiddleware<ACommandWithMiddlewares>),
            typeof(AThirdlyExecutedStronglyTypedCommandMiddleware));
    }

    [Fact]
    public async Task CommandWithResultCorrespondingMiddlewaresAreExecutedInOrder()
    {
        await CommandDispatcher.Dispatch(new ACommandWithResultWithMiddlewares(), TestContext.Current.CancellationToken);
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<ACommandWithResultWithMiddlewares, SomeCommandResult>),
            typeof(ASecondlyExecutedGenericTypedCommandMiddleware<ACommandWithResultWithMiddlewares, SomeCommandResult>),
            typeof(AThirdlyExecutedStronglyTypedCommandWithResultMiddleware));
    }

    [Fact]
    public async Task SomeCommandsMightOnlyHaveGenericMiddlewares()
    {
        await CommandDispatcher.Dispatch(new SomeCommand("Requested by me"), TestContext.Current.CancellationToken);
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SomeCommand>),
            typeof(ASecondlyExecutedGenericTypedCommandMiddleware<SomeCommand>));
    }
}
