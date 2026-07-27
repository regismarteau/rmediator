using Dispatcher.Commands;
using Dispatcher.DependencyInjection;
using Dispatcher.DomainEvents;
using Dispatcher.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.UnitTests;

public class BaseTests
{

    public BaseTests()
    {
        this.Services = Initialize(new ServiceCollection()
            .AddDispatcher(o => o.WithAssemblies(this.GetType().Assembly)))
            .BuildServiceProvider(true);
    }

    public virtual IServiceCollection Initialize(IServiceCollection services) => services;

    protected ServiceProvider Services { get; }

    protected IDispatchQuery QueryDispatcher => GetService<IDispatchQuery>();

    protected IDispatchCommand CommandDispatcher => GetService<IDispatchCommand>();

    protected IPublishDomainEvents DomainEventPublisher => GetService<IPublishDomainEvents>();

    protected T GetService<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }
}