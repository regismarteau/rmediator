using RMediator.Abstractions;

namespace RMediator.DependencyInjection.Extensions;

internal static class MiddlewareExtensions
{
    extension(IReadOnlyCollection<IAbstractMiddlewareHandler> handlers)
    {
        public NextMiddleware Wrap(NextMiddleware finalHandlerCall)
        {
            return handlers.Reverse().Aggregate(
                finalHandlerCall,
                (next, handler) => (request, token) => handler.Handle(request, next, token));
        }
    }

    extension<TResponse>(IReadOnlyCollection<IAbstractMiddlewareHandler<TResponse>> handlers)
    {
        public NextMiddleware<TResponse> Wrap(NextMiddleware<TResponse> finalHandlerCall)
        {
            return handlers.Reverse().Aggregate(
                finalHandlerCall,
                (next, handler) => (request, token) => handler.Handle(request, next, token));
        }
    }
}
