using Dispatcher.UnitTests.Handlers.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using NFluent;

namespace Dispatcher.UnitTests;

public class DomainEventPublisherTests : BaseTests
{
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
        Check.That(GetService<DomainEventListened>().RaisedBy).Is(raisedBy);
    }
}