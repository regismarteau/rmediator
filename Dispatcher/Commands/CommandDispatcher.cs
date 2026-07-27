using Dispatcher.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Commands;

public class CommandDispatcher(HandlersRegistry registry, IServiceProvider services) : IDispatchCommand
{
    public async Task Dispatch<TCommand>(TCommand command, CancellationToken? cancellationToken = null) where TCommand : ICommand
    {
        var handlers = FindHandlersBy(command);
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for command {command.GetType().Name}");
        }

        await ((IHandleCommand<TCommand>)services.GetRequiredService(handlers.First().HandledInterface)).Handle(command, cancellationToken ?? CancellationToken.None);
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null)
    {
        var handlers = FindHandlersBy(command);
        if (handlers.Count != 1)
        {
            throw new InvalidOperationException($"There is no unique handler for command {command.GetType().Name}");
        }

        return (TResult)await ((IHandleRequest)services.GetRequiredService(handlers.First().HandledInterface)).Handle(command, cancellationToken ?? CancellationToken.None);
    }

    private IReadOnlyCollection<HandlerDefinition> FindHandlersBy(object command)
    {
        return [.. registry.CommandHandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(command.GetType()))];
    }
}