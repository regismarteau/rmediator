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
        await DomainEventPublisher.Publish(new SomeDomainEvent(raisedBy));
        Check.That(DomainEvent.RaisedBy).Is(raisedBy);
        Check.That(DomainEvent.ListenedBy).IsEquivalentTo(typeof(SomeDomainEventListener));
    }

    [Fact]
    public async Task MultipleListenersCouldByDefined()
    {
        await DomainEventPublisher.Publish(new AMultipleListenedDomainEvent());
        Check.That(DomainEvent.ListenedBy).IsEquivalentTo(typeof(AFirstListenerHandler), typeof(ASecondListenerHandler));
    }

    [Fact]
    public async Task ListenersHandleDomainEventsInheritance()
    {
        await DomainEventPublisher.Publish(new AnInheritedDomainEvent());
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
        await DomainEventPublisher.Publish(new SomeDomainEvent("Raised by me"));
        Check.That(ExecutedMiddlewares.ExecutedTypes).ContainsExactly(
            typeof(AFirstlyExecutedGeneralMiddleware<SomeDomainEvent>),
            typeof(ASecondlyExecutedGenericTypedDomainEventMiddleware<SomeDomainEvent>),
            typeof(AThirdlyExecutedStronglyTypedDomainEventMiddleware));
    }
}