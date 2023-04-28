#if !NETSTANDARD2_0

namespace Jay.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable,
                                                     CancellationToken token = default)
    {
        var list = new List<T>();
        await foreach (var x in asyncEnumerable.WithCancellation(token))
        {
            list.Add(x);
        }
        return list;
    }
}

#endif