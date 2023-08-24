namespace Jay.Extensions;

public static class SpanExtensions
{
#if !NET6_0_OR_GREATER
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
#endif
}