using System.Reflection;
using System.Runtime.ExceptionServices;
using RMediator.Abstractions;

namespace RMediator.DependencyInjection.Extensions;

internal static class TypeExtensions
{
    extension(Type type)
    {
        public bool IsOfGenericType(params Type[] genericTypes)
        {
            return type.IsGenericType && genericTypes.Contains(type.GetGenericTypeDefinition());
        }

        public bool IsAMiddleware()
        {
            return type.IsOfGenericType(typeof(IHandleMiddleware<,>), typeof(IHandleMiddleware<>));
        }
    }

    extension(object obj)
    {
        public Task HandleGenericRequest(IRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)typeof(IHandleRequest<>)
                    .MakeGenericType(request.GetType())
                    .GetMethod(nameof(IHandleRequest<>.Handle))!
                    .Invoke(obj, [request, cancellationToken])!;
            }
            catch (TargetInvocationException exception) when (exception.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }
    }
}
