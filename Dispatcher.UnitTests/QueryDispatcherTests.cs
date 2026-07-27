using Dispatcher.UnitTests.Handlers.Queries;
using NFluent;

namespace Dispatcher.UnitTests;

public class QueryDispatcherTests : BaseTests
{
    [Fact]
    public async Task ShouldReturnSomeResponse()
    {
        var response = await QueryDispatcher.Dispatch(new SomeQuery());
        Check.That(response.Value).Is("Some response!");
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
}