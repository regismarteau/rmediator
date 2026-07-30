using HumbleMediator.Abstractions;
using HumbleMediator.DependencyInjection;
using HumbleMediator.UnitTests.Handlers.Commands;
using HumbleMediator.UnitTests.Handlers.DomainEvents;
using HumbleMediator.UnitTests.Handlers.Middlewares;
using HumbleMediator.UnitTests.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace HumbleMediator.UnitTests;

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
                    typeof(AThirdlyExecutedStronglyTypedDomainEventMiddleware)))
            .AddSingleton<ExecutedMiddlewares>())
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