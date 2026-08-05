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
Registration go through **Microsoft** classic dependency injection via the method below:

```csharp
services
    .AddMediator(o => o
    .ScanAssemblies(
        typeof(MyClass1).Assembly, 
        typeof(MyClass2).Assembly));
```

Middlewares can also be referenced **in order** here:
```csharp
services
    .AddMediator(o => o
    .AddMiddlewares(
        typeof(MonitorQuery), 
        typeof(CommitAfterCommand)));
```

> [!TIP]
> The method `.AddMediator(options => ...)` can be called multiple times during services registration to allow splitted configuration if needed.

## Implementation
This package allows to split the mediator pattern into the two core concepts of CQRS: command and query

- `IDispatchQuery / IDispatchCommand` to inject in order to dispatch corresponding `IQuery<TResponse> / ICommand / ICommand<TResult>`
- `IHandleQuery<TQuery, TResponse> / IHandleCommand<TCommand> / IHandleCommand<TCommand, TResult>` to define the corresponding handlers
- `IPublishDomainEvent` to inject in order to publish `IDomainEvent`
- `IListenToDomainEvent` to handle synchronously published `IDomainEvent`.
- `IHandleMiddleware<TRequest> / IHandleMiddleware<TRequest, TResponse>` to add specific behavior to given commands / queries / domain events.

> [!TIP]
> All of the handlers support inheritance for more flexibility and behavior factorization.
> Middlewares also support genericity and constraints to fine-tune your pipelines.