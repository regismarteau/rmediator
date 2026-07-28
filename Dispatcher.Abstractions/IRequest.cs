namespace Dispatcher.Abstractions;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;
