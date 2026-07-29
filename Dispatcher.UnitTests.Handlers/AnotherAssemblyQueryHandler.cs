using Dispatcher.Abstractions;

namespace Dispatcher.UnitTests.Handlers;

public record AnotherAssemblyQuery : IQuery<string>;
public class AnotherAssemblyQueryHandler : IHandleQuery<AnotherAssemblyQuery, string>
{
    public Task<string> Handle(AnotherAssemblyQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult("and through another query!");
    }
}

public class AnotherAssemblyMiddleware() : IHandleMiddleware<AnotherAssemblyQuery, string>
{
    public async Task<string> Handle(AnotherAssemblyQuery request, NextMiddleware<string> next, CancellationToken cancellationToken)
    {
        return $"It goes through another middleware {await next(request, cancellationToken)}";
    }
}