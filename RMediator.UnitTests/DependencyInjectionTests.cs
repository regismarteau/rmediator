using Microsoft.Extensions.DependencyInjection;
using NFluent;
using RMediator.Abstractions;
using RMediator.DependencyInjection;
using RMediator.UnitTests.Handlers;
using RMediator.UnitTests.Handlers.Middlewares;
using RMediator.UnitTests.Handlers.Queries;

namespace RMediator.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task ItIsPossibleToConcatMultipleConfigurations()
    {
        var services = new ServiceCollection()
            .AddMediator(o => o.ScanAssemblies(typeof(DependencyInjectionTests).Assembly).AddMiddlewares(typeof(AFirstlyExecutedGeneralMiddleware<>)))
            .AddMediator(o => o.ScanAssemblies(typeof(AnotherAssemblyQuery).Assembly).AddMiddlewares(typeof(AnotherAssemblyMiddleware)))
            .BuildServiceProvider(true);

        var queryDispatcher = services.GetRequiredService<IDispatchQuery>();
        var someQueryResponse = await queryDispatcher.Dispatch(new SomeQuery("me"));
        var anotherQueryResponse = await queryDispatcher.Dispatch(new AnotherAssemblyQuery());

        Check.That(someQueryResponse.Value).Is("Some response for me!");
        Check.That(anotherQueryResponse).Is("It goes through another middleware and through another query!");
    }

    [Fact]
    public async Task AbstractHandlersAreIgnoredWhenScanningAssemblies()
    {
        var services = new ServiceCollection()
            .AddMediator(o => o.ScanAssemblies(typeof(DependencyInjectionTests).Assembly))
            .BuildServiceProvider(true);

        var response = await services.GetRequiredService<IDispatchQuery>().Dispatch(new ConcreteHandledQuery());

        Check.That(response).Is("from concrete handler");
    }
}

public record ConcreteHandledQuery : IQuery<string>;

public abstract class AbstractQueryHandler : IHandleQuery<ConcreteHandledQuery, string>
{
    public abstract Task<string> Handle(ConcreteHandledQuery query, CancellationToken cancellationToken);
}

public class ConcreteQueryHandler : AbstractQueryHandler
{
    public override Task<string> Handle(ConcreteHandledQuery query, CancellationToken cancellationToken) => Task.FromResult("from concrete handler");
}
