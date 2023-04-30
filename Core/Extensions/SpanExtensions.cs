namespace Jay.Extensions;

public static class SpanExtensions
{
#if NETSTANDARD2_0 || NETSTANDARD2_1
    public static bool SequenceEqual<T>(this Span<T> first, Span<T> second, IEqualityComparer<T>? itemComparer = null)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }

    public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer = null)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, Span<T> second, IEqualityComparer<T>? itemComparer = null)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer = null)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }
    #else
    // public static bool SequenceEqual<T>(this Span<T> first, Span<T> second, IEqualityComparer<T>? itemComparer = null)
    //     => MemoryExtensions.SequenceEqual(first, second, itemComparer);
    // public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer = null)
    //     => MemoryExtensions.SequenceEqual(first, second, itemComparer);
    // public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, Span<T> second, IEqualityComparer<T>? itemComparer = null)
    //     => MemoryExtensions.SequenceEqual(first, second, itemComparer);
    // public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer = null)
    //     => MemoryExtensions.SequenceEqual(first, second, itemComparer);
#endif
}