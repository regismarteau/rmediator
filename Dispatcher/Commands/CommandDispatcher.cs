using Dispatcher.DependencyInjection;
using Dispatcher.Requests;

namespace Dispatcher.Commands;

internal class CommandDispatcher(HandlersRegistry registry, IServiceProvider services) : BaseRequestDispatcher(registry, services), IDispatchCommand
{
    public async Task Dispatch<TCommand>(TCommand command, CancellationToken? cancellationToken = null) where TCommand : ICommand
    {
        await SendRequest(command, GetSingleHandler(command), cancellationToken ?? CancellationToken.None);
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null)
    {
        return await SendRequest(command, GetSingleHandler(command), cancellationToken ?? CancellationToken.None);
    }
}