using HumbleMediator.Abstractions;

namespace HumbleMediator.UnitTests.Handlers.Queries;

public record ACancellableQuery : IQuery<ACancellableQueryResponse>;

public record ACancellableQueryResponse(bool HasBeenCanceled);

public class ACancellableQueryHandler : IHandleQuery<ACancellableQuery, ACancellableQueryResponse>
{
    public async Task<ACancellableQueryResponse> Handle(ACancellableQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var semaphore = new SemaphoreSlim(0);
            await semaphore.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return new(true);
        }

        return new(false);
    }
}