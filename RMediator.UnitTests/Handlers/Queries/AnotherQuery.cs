using RMediator.Abstractions;

namespace RMediator.UnitTests.Handlers.Queries;

public record AnotherQuery : IQuery<SomeQueryResponse>;

public class AnotherQueryHandler : IHandleQuery<AnotherQuery, SomeQueryResponse>
{
    public Task<SomeQueryResponse> Handle(AnotherQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeQueryResponse("Another response!"));
    }
}