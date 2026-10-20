namespace RMediator.Abstractions;

/// <summary>Dispatches commands to their single handler through the middleware pipeline.</summary>
public interface IDispatchCommand
{
    /// <summary>Dispatches a command without result.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="InvalidOperationException">No handler, or several handlers, are registered.</exception>
    Task Dispatch(ICommand command, CancellationToken cancellationToken = default);
    /// <summary>Dispatches a command with a result.</summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    /// <exception cref="InvalidOperationException">No handler, or several handlers, are registered.</exception>
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}

/// <summary>A request that changes state, without result.</summary>
public interface ICommand : IRequest;

/// <summary>A request that changes state and returns a result.</summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface ICommand<TResult> : IRequest<TResult>;

/// <summary>Handles a command without result. A command must have exactly one handler.</summary>
/// <typeparam name="TCommand">The type of command.</typeparam>
public interface IHandleCommand<in TCommand> : IHandleRequest<TCommand> where TCommand : ICommand;

/// <summary>Handles a command with a result. A command must have exactly one handler.</summary>
/// <typeparam name="TCommand">The type of command.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IHandleCommand<in TCommand, TResult> : IHandleRequestWithResponse<TResult> where TCommand : ICommand<TResult>
{
    /// <summary>Handles the command.</summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);

    async Task<TResult> IHandleRequestWithResponse<TResult>.Handle(IRequest<TResult> request, CancellationToken cancellationToken)
    {
        return await Handle((TCommand)request, cancellationToken);
    }
}
