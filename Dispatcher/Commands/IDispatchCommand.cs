using Dispatcher.Requests;

namespace Dispatcher.Commands;

public interface IDispatchCommand
{
    Task Dispatch<TCommand>(TCommand command, CancellationToken? cancellationToken = null) where TCommand : ICommand;
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null);
}

public interface ICommand : IRequest;

public interface ICommand<TResult> : IRequest<TResult>;

public interface IHandleCommand<in TCommand> : IHandleRequest where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken cancellationToken);

    async Task IHandleRequest.Handle(IRequest request, CancellationToken cancellationToken)
    {
        await Handle((TCommand)request, cancellationToken);
    }
}

public interface IHandleCommand<in TCommand, TResult> : IHandleRequest<TResult> where TCommand : ICommand<TResult>
{
    public Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);

    async Task<TResult> IHandleRequest<TResult>.Handle(IRequest<TResult> request, CancellationToken cancellationToken)
    {
        return await Handle((TCommand)request, cancellationToken);
    }
}