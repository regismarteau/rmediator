using HumbleMediator.Abstractions;
using HumbleMediator.DependencyInjection;
using HumbleMediator.UnitTests.Handlers;
using HumbleMediator.UnitTests.Handlers.Middlewares;
using HumbleMediator.UnitTests.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using NFluent;

namespace HumbleMediator.UnitTests;

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
