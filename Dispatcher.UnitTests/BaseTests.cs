using Dispatcher.Commands;
using Dispatcher.DependencyInjection;
using Dispatcher.DomainEvents;
using Dispatcher.Queries;
using Dispatcher.UnitTests.Handlers.Commands;
using Dispatcher.UnitTests.Handlers.DomainEvents;
using Dispatcher.UnitTests.Handlers.Middlewares;
using Dispatcher.UnitTests.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.UnitTests;

public class BaseTests
{
    public BaseTests()
    {
        Services = Initialize(new ServiceCollection()
            .AddDispatcher(o => o
                .ScanAssemblies(GetType().Assembly)
                .AddMiddlewares(
                    typeof(AFirstlyExecutedGeneralMiddleware<>),
                    typeof(AFirstlyExecutedGeneralMiddleware<,>),
                    typeof(ASecondlyExecutedGenericTypedQueryMiddleware<,>),
                    typeof(ASecondlyExecutedGenericTypedCommandMiddleware<>),
                    typeof(ASecondlyExecutedGenericTypedCommandMiddleware<,>),
                    typeof(ASecondlyExecutedGenericTypedDomainEventMiddleware<>),
                    typeof(AThirdlyExecutedStronglyTypedQueryMiddleware),
                    typeof(AThirdlyExecutedStronglyTypedCommandMiddleware),
                    typeof(AThirdlyExecutedStronglyTypedCommandWithResultMiddleware),
                    typeof(AThirdlyExecutedStronglyTypedDomainEventMiddleware)))
            .AddSingleton<ExecutedMiddlewares>())
            .BuildServiceProvider(true);
    }

    public virtual IServiceCollection Initialize(IServiceCollection services) => services;

    protected ServiceProvider Services { get; }

    protected IDispatchQuery QueryDispatcher => GetService<IDispatchQuery>();

    protected IDispatchCommand CommandDispatcher => GetService<IDispatchCommand>();

    protected IPublishDomainEvents DomainEventPublisher => GetService<IPublishDomainEvents>();

    protected ExecutedMiddlewares ExecutedMiddlewares => GetService<ExecutedMiddlewares>();

    protected T GetService<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }
}