namespace Dispatcher.Requests;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;
