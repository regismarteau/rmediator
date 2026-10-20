using Microsoft.Extensions.DependencyInjection;
using NFluent;
using RMediator.UnitTests.Handlers.DomainEvents;
using RMediator.UnitTests.Handlers.Middlewares;

namespace RMediator.UnitTests;

public class DomainEventPublisherTests : BaseTests
{
    private DomainEventListened DomainEvent => GetService<DomainEventListened>();
    public override IServiceCollection Initialize(IServiceCollection services)
    {
        return base.Initialize(services)
            .AddSingleton<DomainEventListened>();
    }

    [Theory]
    [InlineData("someone")]
    [InlineData("another guy")]
    public async Task ShouldListenToDomainEvent(string raisedBy)
    {
        await DomainEventPublisher.Publish(new SomeDomainEvent(raisedBy), TestContext.Current.CancellationToken);
        Check.That(DomainEvent.RaisedBy).Is(raisedBy);
        Check.That(DomainEvent.ListenedBy).IsEquivalentTo(typeof(SomeDomainEventListener));
    }

    [Fact]
    public async Task MultipleListenersCouldByDefined()
    {
        await DomainEventPublisher.Publish(new AMultipleListenedDomainEvent(), TestContext.Current.CancellationToken);
        Check.That(DomainEvent.ListenedBy).IsEquivalentTo(typeof(AFirstListenerHandler), typeof(ASecondListenerHandler));
    }

    [Fact]
    public async Task ListenersHandleDomainEventsInheritance()
    {
        await DomainEventPublisher.Publish(new AnInheritedDomainEvent(), TestContext.Current.CancellationToken);
        Check.That(DomainEvent.ListenedBy).IsEquivalentTo(
            typeof(AFirstStronglyTypedTwiceListenerHandler),
            typeof(AFirstStronglyTypedTwiceListenerHandler),
            typeof(ASecondLessStrictConcreteTypeListenerHandler),
            typeof(AThirdInheritedInterfaceListenerHandler),
            typeof(AFourthGenericListenerHandler<AnInheritedDomainEvent>));
    }

    [Fact]
    public async Task DomainEventCorrespondingMiddlewaresAreExecutedInOrder()
    {
        await DomainEventPublisher.Publish(new SomeDomainEvent("Raised by me"), TestContext.Current.CancellationToken);
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SomeDomainEvent>),
            typeof(ASecondlyExecutedGenericTypedDomainEventMiddleware<SomeDomainEvent>),
            typeof(AThirdlyExecutedStronglyTypedDomainEventMiddleware));
    }
}
