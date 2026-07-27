using Dispatcher.DependencyInjection;

namespace Dispatcher.Commands;

public interface IDispatchCommand
{
    Task Dispatch<TCommand>(TCommand command, CancellationToken? cancellationToken = null) where TCommand : ICommand;
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null);
}

public interface ICommand;

public interface ICommand<TResult>;

public interface IHandleCommand<TCommand> where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken cancellationToken);
}

public interface IHandleCommand<TCommand, TResult> : IHandleRequest where TCommand : ICommand<TResult> where TResult : notnull
{
    public Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);

    async Task<object> IHandleRequest.Handle(object request, CancellationToken cancellationToken)
    {
        return await this.Handle((TCommand)request, cancellationToken);
    }
}