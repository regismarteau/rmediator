# RMediator
As the famous MediatR package has turned into commercial, like many other people I propose my lightweight version of what MediatR offered the best over time.

Nothing fancy here, just basic stuff with the key features I've used the most and I still need to use in my different projects.

# Install
## RMediator.DependencyInjection
The all-in-one package:

    dotnet add package RMediator.DependencyInjection

## RMediator.Abstractions
A no-dependency package only referencing usable interfaces in order to keep core domain as pure and clean as possible:

    dotnet add package RMediator.Abstractions

# How to use it
## Registration
Registration goes through **Microsoft** classic dependency injection via the method below:

```csharp
services
    .AddMediator(o => o
    .ScanAssemblies(
        typeof(MyClass1).Assembly, 
        typeof(MyClass2).Assembly));
```

Middlewares can also be referenced here. They are executed **in the order** they are registered:
```csharp
services
    .AddMediator(o => o
    .AddMiddlewares(
        typeof(MonitorQuery), 
        typeof(CommitAfterCommand)));
```

> [!TIP]
> The method `.AddMediator(options => ...)` can be called multiple times during services registration to allow split configuration if needed.

## Implementation
This package allows to split the mediator pattern into the two core concepts of CQRS: command and query

- `IDispatchQuery / IDispatchCommand` to inject in order to dispatch corresponding `IQuery<TResponse> / ICommand / ICommand<TResult>`
- `IHandleQuery<TQuery, TResponse> / IHandleCommand<TCommand> / IHandleCommand<TCommand, TResult>` to define the corresponding handlers
- `IPublishDomainEvent` to inject in order to publish `IDomainEvent`
- `IListenToDomainEvent` to handle synchronously published `IDomainEvent`.
- `IHandleMiddleware<TRequest> / IHandleMiddleware<TRequest, TResponse>` to add specific behavior to given commands / queries / domain events.

> [!TIP]
> Handlers and middlewares support inheritance for more flexibility and behavior factorization: a middleware targeting a base type (or an interface) applies to all of its sub-types.
> Middlewares also support genericity and constraints to fine-tune your pipelines.

## Behavior notes
- Domain events are published synchronously: `Publish` waits for the listeners, which run one after the other. The first listener exception stops the following ones and is thrown to the caller.
- Middlewares of a domain event are only executed when at least one listener handles it.
- Exceptions thrown by handlers are propagated as is, never wrapped.