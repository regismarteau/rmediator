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
}
