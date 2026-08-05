namespace RMediator.Extensions;

internal static class ListExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> elements, Func<T, Task> func)
    {
        foreach (var element in elements)
        {
            await func(element);
        }
    }
}