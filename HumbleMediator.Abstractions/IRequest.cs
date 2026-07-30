namespace HumbleMediator.Abstractions;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;
