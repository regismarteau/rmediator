using Microsoft.Extensions.DependencyInjection;
using RMediator.Abstractions;
using RMediator.DependencyInjection;
using RMediator.UnitTests.Handlers.Commands;
using RMediator.UnitTests.Handlers.DomainEvents;
using RMediator.UnitTests.Handlers.Middlewares;
using RMediator.UnitTests.Handlers.Queries;

namespace RMediator.UnitTests;

public class BaseTests
{
    public BaseTests()
    {
        Services = Initialize(new ServiceCollection()
            .AddMediator(o => o
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
                    typeof(AThirdlyExecutedStronglyTypedDomainEventMiddleware),
                    typeof(BaseCommandMiddleware),
                    typeof(BaseCommandWithResultMiddleware),
                    typeof(BaseQueryMiddleware),
                    typeof(BaseEventMiddleware)))
            .AddSingleton<ExecutedMiddlewares>()
            .AddSingleton<EventProbe>())
            .BuildServiceProvider(true);
    }

    public virtual IServiceCollection Initialize(IServiceCollection services) => services;

    protected ServiceProvider Services { get; }

    protected IDispatchQuery QueryDispatcher => GetService<IDispatchQuery>();

    protected IDispatchCommand CommandDispatcher => GetService<IDispatchCommand>();

    protected IPublishDomainEvent DomainEventPublisher => GetService<IPublishDomainEvent>();

    protected ExecutedMiddlewares ExecutedMiddlewares => GetService<ExecutedMiddlewares>();

    protected T GetService<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }
}