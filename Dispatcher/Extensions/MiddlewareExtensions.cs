using Dispatcher.Middlewares;

namespace Dispatcher.Extensions;

internal static class MiddlewareExtensions
{
    extension(IReadOnlyCollection<IAbstractMiddlewareHandler> handlers)
    {
        public NextMiddleware Wrap(NextMiddleware finalHandlerCall)
        {
            NextMiddleware GetNextMiddleware(int index)
            {
                if (index == handlers.Count - 1)
                {
                    return finalHandlerCall;
                }
                return async (request, token) => await handlers.ElementAt(index + 1).Handle(request, GetNextMiddleware(index + 1), token);
            }

            if (handlers.Count > 0)
            {
                return (request, token) => handlers.First().Handle(request, GetNextMiddleware(0), token);
            }

            return finalHandlerCall;
        }
    }

    extension<TResponse>(IReadOnlyCollection<IAbstractMiddlewareHandler<TResponse>> handlers)
    {
        public NextMiddleware<TResponse> Wrap(NextMiddleware<TResponse> finalHandlerCall)
        {
            NextMiddleware<TResponse> GetNextMiddleware(int index)
            {
                if (index == handlers.Count - 1)
                {
                    return finalHandlerCall;
                }
                return async (request, token) => await handlers.ElementAt(index + 1).Handle(request, GetNextMiddleware(index + 1), token);
            }

            if (handlers.Count > 0)
            {
                return (request, token) => handlers.First().Handle(request, GetNextMiddleware(0), token);
            }

            return finalHandlerCall;
        }
    }
}