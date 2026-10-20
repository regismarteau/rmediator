# RMediator

[![CI](https://github.com/regismarteau/rmediator/actions/workflows/ci.yml/badge.svg)](https://github.com/regismarteau/rmediator/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/RMediator.DependencyInjection.svg)](https://www.nuget.org/packages/RMediator.DependencyInjection)
[![Downloads](https://img.shields.io/nuget/dt/RMediator.DependencyInjection.svg)](https://www.nuget.org/packages/RMediator.DependencyInjection)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A lightweight, free and open source mediator for .NET, built around the features of MediatR that matter most: **commands**, **queries**, **domain events** and a **middleware pipeline**.

Nothing fancy, just the basics, with a small API and no magic.

- Explicit CQRS vocabulary: `IDispatchCommand`, `IDispatchQuery`, `IPublishDomainEvent`
- Middlewares (pipeline behaviors) with ordering, generics, constraints and inheritance
- Registration through the standard `Microsoft.Extensions.DependencyInjection`
- Targets .NET 8, .NET 9 and .NET 10
- A contracts-only package to keep your domain free of any infrastructure dependency

## Install

The all-in-one package:

```sh
dotnet add package RMediator.DependencyInjection
```

A package that only contains the interfaces, with no dependency, to reference from your domain or application layers:

```sh
dotnet add package RMediator.Abstractions
```

## Quick start

Define a command and its handler:

```csharp
using RMediator.Abstractions;

public record CreateOrder(string Customer) : ICommand<Guid>;

public class CreateOrderHandler(IOrders orders) : IHandleCommand<CreateOrder, Guid>
{
    public async Task<Guid> Handle(CreateOrder command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        await orders.Add(id, command.Customer, cancellationToken);
        return id;
    }
}
```

Register the mediator by scanning the assemblies that contain your handlers:

```csharp
services.AddMediator(options => options
    .ScanAssemblies(typeof(CreateOrderHandler).Assembly));
```

Inject a dispatcher and use it:

```csharp
public class OrdersController(IDispatchCommand commands, IDispatchQuery queries)
{
    public Task<Guid> Create(string customer, CancellationToken cancellationToken)
    {
        return commands.Dispatch(new CreateOrder(customer), cancellationToken);
    }
}
```

## Concepts

| You want to... | Message | Handler | Send it with |
| --- | --- | --- | --- |
| Change state | `ICommand` / `ICommand<TResult>` | `IHandleCommand<TCommand>` / `IHandleCommand<TCommand, TResult>` | `IDispatchCommand.Dispatch` |
| Read state | `IQuery<TResponse>` | `IHandleQuery<TQuery, TResponse>` | `IDispatchQuery.Dispatch` |
| Announce that something happened | `IDomainEvent` | `IListenToDomainEvent<TEvent>` (any number) | `IPublishDomainEvent.Publish` |
| Add behavior around any of the above | - | `IHandleMiddleware<TRequest>` / `IHandleMiddleware<TRequest, TResponse>` | registered with `AddMiddlewares` |

A command or a query must have **exactly one** handler. A domain event can have **zero or many** listeners.

### Queries

```csharp
public record GetOrder(Guid Id) : IQuery<OrderDto>;

public class GetOrderHandler(IOrders orders) : IHandleQuery<GetOrder, OrderDto>
{
    public Task<OrderDto> Handle(GetOrder query, CancellationToken cancellationToken)
    {
        return orders.Get(query.Id, cancellationToken);
    }
}

var order = await queries.Dispatch(new GetOrder(id), cancellationToken);
```

### Domain events

```csharp
public record OrderCreated(Guid OrderId) : IDomainEvent;

public class SendConfirmationEmail(IMailer mailer) : IListenToDomainEvent<OrderCreated>
{
    public Task Listen(OrderCreated @event, CancellationToken cancellationToken)
    {
        return mailer.SendConfirmation(@event.OrderId, cancellationToken);
    }
}

await events.Publish(new OrderCreated(id), cancellationToken);
```

### Middlewares

A middleware wraps the handler and decides whether and when to call the next step:

```csharp
public class LoggingMiddleware<TRequest, TResponse>(ILogger<LoggingMiddleware<TRequest, TResponse>> logger)
    : IHandleMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, NextMiddleware<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
        return await next(request, cancellationToken);
    }
}
```

Middlewares are opted in explicitly and run **in the order they are registered**:

```csharp
services.AddMediator(options => options
    .ScanAssemblies(typeof(CreateOrderHandler).Assembly)
    .AddMiddlewares(
        typeof(LoggingMiddleware<,>),
        typeof(CommitAfterCommand)));
```

- `IHandleMiddleware<TRequest, TResponse>` applies to commands with a result and to queries, `IHandleMiddleware<TRequest>` to commands without a result and to domain events.
- A middleware targeting a base type or an interface applies to all of its sub-types.
- Open generic middlewares and generic constraints let you target a family of requests, for example only commands.

### Registration

`AddMediator` can be called several times, for example once per module, and the configurations are merged:

```csharp
services
    .AddMediator(options => options.ScanAssemblies(typeof(SalesModule).Assembly))
    .AddMediator(options => options.ScanAssemblies(typeof(BillingModule).Assembly));
```

Handlers and middlewares are registered as transient services, so they can depend on anything registered in your container. Abstract classes and interfaces are ignored when scanning.

## Behavior notes

- Domain events are published synchronously: `Publish` waits for the listeners, which run one after the other. The first listener exception stops the following ones and is thrown to the caller.
- The middlewares of a domain event only run when at least one listener handles it.
- Exceptions thrown by handlers are propagated as is, never wrapped.
- Handlers support inheritance: a handler for a base type also handles its sub-types.

## Coming from MediatR

| MediatR | RMediator |
| --- | --- |
| `IRequest<T>` + `IRequestHandler` (command) | `ICommand<T>` + `IHandleCommand<TCommand, T>` |
| `IRequest<T>` + `IRequestHandler` (query) | `IQuery<T>` + `IHandleQuery<TQuery, T>` |
| `INotification` + `INotificationHandler` | `IDomainEvent` + `IListenToDomainEvent` |
| `IPipelineBehavior<TRequest, TResponse>` | `IHandleMiddleware<TRequest, TResponse>` |
| `IMediator.Send` / `IMediator.Publish` | `IDispatchCommand` / `IDispatchQuery` / `IPublishDomainEvent` |
| `services.AddMediatR(...)` | `services.AddMediator(...)` |

Not supported: a single `IMediator` entry point, stream requests, pre/post processors, exception handlers and custom notification publishers. If you rely on them, RMediator is probably not the right fit.

## Contributing

Contributions are welcome, see [CONTRIBUTING.md](CONTRIBUTING.md). Release notes are in [CHANGELOG.md](CHANGELOG.md). To report a vulnerability, see [SECURITY.md](SECURITY.md).

## License

[MIT](LICENSE)
