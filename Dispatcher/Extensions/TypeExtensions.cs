using Dispatcher.Abstractions;

namespace Dispatcher.Extensions;

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
}
