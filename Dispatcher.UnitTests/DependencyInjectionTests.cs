using Dispatcher.Abstractions;
using Dispatcher.DependencyInjection;
using Dispatcher.UnitTests.Handlers;
using Dispatcher.UnitTests.Handlers.Middlewares;
using Dispatcher.UnitTests.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using NFluent;

namespace Dispatcher.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task ItIsPossibleToConcatMultipleConfigurations()
    {
        var services = new ServiceCollection()
            .AddDispatcher(o => o.ScanAssemblies(typeof(DependencyInjectionTests).Assembly).AddMiddlewares(typeof(AFirstlyExecutedGeneralMiddleware<>)))
            .AddDispatcher(o => o.ScanAssemblies(typeof(AnotherAssemblyQuery).Assembly).AddMiddlewares(typeof(AnotherAssemblyMiddleware)))
            .BuildServiceProvider(true);

        var queryDispatcher = services.GetRequiredService<IDispatchQuery>();
        var someQueryResponse = await queryDispatcher.Dispatch(new SomeQuery("me"));
        var anotherQueryResponse = await queryDispatcher.Dispatch(new AnotherAssemblyQuery());

        Check.That(someQueryResponse.Value).Is("Some response for me!");
        Check.That(anotherQueryResponse).Is("It goes through another middleware and through another query!");
    }
}
