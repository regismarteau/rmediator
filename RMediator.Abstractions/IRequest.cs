namespace RMediator.Abstractions;

/// <summary>Marker for any message handled by the mediator.</summary>
public interface IRequest;

/// <summary>Marker for a message that produces a response.</summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<out TResponse> : IRequest;
