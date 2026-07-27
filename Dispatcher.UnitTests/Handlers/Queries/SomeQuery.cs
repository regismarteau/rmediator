using Dispatcher.Queries;

namespace Dispatcher.UnitTests.Handlers.Queries;

public record SomeQuery(string RequestedBy) : IQuery<SomeQueryResponse>;

public record SomeQueryResponse(string Value);

public class SomeQueryHandler : IHandleQuery<SomeQuery, SomeQueryResponse>
{
    public Task<SomeQueryResponse> Handle(SomeQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeQueryResponse($"Some response for {query.RequestedBy}!"));
    }
}