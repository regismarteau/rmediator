using NFluent;

namespace Dispatcher.UnitTests;

public class DependencyInjectionTests : BaseTests
{
    [Fact]
    public void ShouldResolveIDispatchQuery()
    {
        Check.That(QueryDispatcher).IsNotNull();
    }

    [Fact]
    public void ShouldResolveIDispatchCommand()
    {
        Check.That(CommandDispatcher).IsNotNull();
    }

    [Fact]
    public void ShouldResolveIPublishCommand()
    {
        Check.That(DomainEventPublisher).IsNotNull();
    }
}
