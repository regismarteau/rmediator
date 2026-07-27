using Dispatcher.UnitTests.Handlers.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using NFluent;

namespace Dispatcher.UnitTests;

public class DomainEventPublisherTests : BaseTests
{
    private DomainEventListened DomainEvents => GetService<DomainEventListened>();
    public override IServiceCollection Initialize(IServiceCollection services)
    {
        return base.Initialize(services)
            .AddSingleton<DomainEventListened>();
    }

    [Theory]
    [InlineData("someone")]
    [InlineData("another one")]
    public async Task ShouldListenToDomainEvent(string raisedBy)
    {
        await this.DomainEventPublisher.Publish(new SomeDomainEvent(raisedBy));
        Check.That(DomainEvents.RaisedBy).Is(raisedBy);
        Check.That(DomainEvents.ListenedBy).IsEquivalentTo(typeof(SomeDomainEventListener));
    }

    [Fact]
    public async Task MultipleListenersCouldByDefined()
    {
        await this.DomainEventPublisher.Publish(new AMultipleListenedDomainEvent());
        Check.That(DomainEvents.ListenedBy).IsEquivalentTo(typeof(AFirstListenerHandler), typeof(ASecondListenerHandler));
    }
}