using Dispatcher.UnitTests.Handlers.Middlewares;
using Dispatcher.UnitTests.Handlers.Queries;
using NFluent;

namespace Dispatcher.UnitTests;

public class QueryDispatcherTests : BaseTests
{
    [Theory]
    [InlineData("someone")]
    [InlineData("another guy")]
    public async Task ShouldReturnSomeResponse(string requestedBy)
    {
        var response = await QueryDispatcher.Dispatch(new SomeQuery(requestedBy));
        Check.That(response.Value).Is($"Some response for {requestedBy}!");
    }

    [Fact]
    public async Task ASubQueryCanTriggerAnHandler()
    {
        var response = await QueryDispatcher.Dispatch(new SomeSubQuery("me"));
        Check.That(response.Value).Is("Some response for me!");
    }

    [Fact]
    public async Task ShouldReturnAnotherResponse()
    {
        var response = await QueryDispatcher.Dispatch(new AnotherQuery());
        Check.That(response.Value).Is("Another response!");
    }

    [Fact]
    public async Task CouldBeCanceled()
    {
        var token = new CancellationTokenSource();
        ACancellableQueryResponse? response = null;
        await Task.WhenAll(
            Task.Run(async () => response = await QueryDispatcher.Dispatch(new ACancellableQuery(), token.Token), TestContext.Current.CancellationToken),
            Task.Factory.StartNew(token.Cancel, TestContext.Current.CancellationToken));

        Check.That(response?.HasBeenCanceled).Is(true);
    }

    [Fact]
    public async Task CorrespondingMiddlewaresAreExecutedInOrder()
    {
        await QueryDispatcher.Dispatch(new AQueryWithMiddlewares());
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<AQueryWithMiddlewares, SomeQueryResponse>),
            typeof(ASecondlyExecutedGenericTypedQueryMiddleware<AQueryWithMiddlewares, SomeQueryResponse>),
            typeof(AThirdlyExecutedStronglyTypedQueryMiddleware));
    }

    [Fact]
    public async Task SomeQueriesMightOnlyHaveGenericMiddlewares()
    {
        await QueryDispatcher.Dispatch(new AnotherQuery());
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<AnotherQuery, SomeQueryResponse>),
            typeof(ASecondlyExecutedGenericTypedQueryMiddleware<AnotherQuery, SomeQueryResponse>));
    }
}