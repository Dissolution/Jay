using System.Diagnostics;

namespace Jay.Extensions;

public static class SpanExtensions
{
    public delegate void RefItem<T>(ref T item);

    public static void ForEach<T>(this Span<T> span, RefItem<T> perItem)
    {
        for (var i = 0; i < span.Length; i++)
        {
            perItem(ref span[i]);
        }
    }

    public static void ExpandAt<T>(this Span<T> span, int index)
    {
        Throw.Index(span.Length, index, true);
        var source = span[index..^1];
        var dest = span[(index + 1)..];
        Debug.Assert(source.Length == dest.Length);
        source.CopyTo(dest);
    }
    
    public static void ExpandAt<T>(this Span<T> span, Range range)
    {
        var (offset, length) = Throw.Range(span.Length, range);
        var source = span[offset..^length];
        var dest = span[(offset + length)..];
        Debug.Assert(source.Length == dest.Length);
        source.CopyTo(dest);
    }
    
    public static void RemoveAt<T>(this Span<T> span, int index)
    {
        Throw.Index(span.Length, index);
        var leftSide = span.Slice(index);
        var rightSide = span.Slice(index + 1);
        rightSide.CopyTo(leftSide);
    }

    public static void RemoveRange<T>(this Span<T> span, int offset, int length)
    {
        Throw.Range(span.Length, offset, length);
        var leftSide = span.Slice(offset);
        var rightSide = span.Slice(offset + length);
        rightSide.CopyTo(leftSide);
    }
    
    public static void RemoveRange<T>(this Span<T> span, Range range)
    {
        var (offset, length) = Throw.Range(span.Length, range);
        var leftSide = span.Slice(offset);
        var rightSide = span.Slice(offset + length);
        rightSide.CopyTo(leftSide);
    }
    
    
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