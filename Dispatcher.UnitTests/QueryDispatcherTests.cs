using Dispatcher.UnitTests.Handlers.Queries;
using NFluent;

namespace Dispatcher.UnitTests;

public class QueryDispatcherTests : BaseTests
{
    [Theory]
    [InlineData("someone")]
    [InlineData("another one")]
    public async Task ShouldReturnSomeResponse(string requestedBy)
    {
        var response = await QueryDispatcher.Dispatch(new SomeQuery(requestedBy));
        Check.That(response.Value).Is($"Some response for {requestedBy}!");
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
            Task.Factory.StartNew(async () => response = await QueryDispatcher.Dispatch(new ACancellableQuery(), token.Token), TestContext.Current.CancellationToken),
            Task.Factory.StartNew(token.Cancel, TestContext.Current.CancellationToken));
        Check.That(response?.HasBeenCanceled ?? false).IsTrue();
    }

    [Fact]
    public async Task CorrespondingMiddlewaresAreExecuted()
    {
        await this.QueryDispatcher.Dispatch(new AQueryWithMiddlewares());
        Check.That(ExecutedMiddlewares.ExecutedTypes).IsEquivalentTo(
            typeof(AFirstStronglyTypedQueryMiddleware), 
            typeof(ASecondGenericTypedQueryMiddleware<AQueryWithMiddlewares, SomeQueryResponse>));
    }
}