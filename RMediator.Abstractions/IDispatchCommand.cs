namespace RMediator.Abstractions;

public interface IDispatchCommand
{
    Task Dispatch(ICommand command, CancellationToken? cancellationToken = null);
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null);
}

public interface ICommand : IRequest;

public interface ICommand<TResult> : IRequest<TResult>;

public interface IHandleCommand<in TCommand> : IHandleRequest<TCommand> where TCommand : ICommand;

public interface IHandleCommand<in TCommand, TResult> : IHandleRequestWithResponse<TResult> where TCommand : ICommand<TResult>
{
    public Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);

    async Task<TResult> IHandleRequestWithResponse<TResult>.Handle(IRequest<TResult> request, CancellationToken cancellationToken)
    {
        return await Handle((TCommand)request, cancellationToken);
    }
}