using Dispatcher.DependencyInjection;
using Dispatcher.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Commands;

public class CommandDispatcher(HandlersRegistry registry, IServiceProvider services) : IDispatchCommand
{
    public async Task Dispatch<TCommand>(TCommand command, CancellationToken? cancellationToken = null) where TCommand : ICommand
    {
        var handlers = registry.CommandHandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(command.GetType())).ToList();
        if (handlers.Count == 1)
        {
            await ((IHandleCommand<TCommand>)services.GetRequiredService(handlers.First().HandledInterface)).Handle(command, cancellationToken ?? CancellationToken.None);
            return;
        }

        throw new InvalidOperationException($"There is no unique handler for command {command.GetType().Name}");
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null)
    {
        var handlers = registry.CommandHandlerDefinitions.Where(t => t.HandledType.IsAssignableFrom(command.GetType())).ToList();
        if (handlers.Count == 1)
        {
            return (TResult)await((IHandleRequest)services.GetRequiredService(handlers.First().HandledInterface)).Handle(command, cancellationToken ?? CancellationToken.None);
        }

        throw new InvalidOperationException($"There is no unique handler for command {command.GetType().Name}");
    }
}