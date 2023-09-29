// ReSharper disable InvokeAsExtensionMethod

using Jay.Comparison;

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

// ReSharper disable once CheckNamespace
namespace Jay;

public static class Easy
{
#region CopyTo
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, T[]? dest)
        => source.AsSpan()
            .CopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, Span<T> dest)
        => source.AsSpan()
            .CopyTo(dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(Span<T> source, T[]? dest)
        => source.CopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(Span<T> source, Span<T> dest)
        => source.CopyTo(dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(ReadOnlySpan<T> source, T[]? dest)
        => source.CopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(ReadOnlySpan<T> source, Span<T> dest)
        => source.CopyTo(dest);
#endregion

#region TryCopyTo
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, T[]? dest)
        => source.AsSpan().TryCopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, Span<T> dest)
        => source.AsSpan().TryCopyTo(dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(Span<T> source, T[]? dest)
        => source.TryCopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(Span<T> source, Span<T> dest)
        => source.TryCopyTo(dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(ReadOnlySpan<T> source, T[]? dest)
        => source.TryCopyTo(dest.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(ReadOnlySpan<T> source, Span<T> dest)
        => source.TryCopyTo(dest);
#endregion

    public static bool ObjEqual(object? left, object? right)
        => EqualityComparerCache.Equals(left, right);

#if NET6_0_OR_GREATER
    public static bool SeqEqual<T>(T[]? left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(T[]? left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(Span<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(Span<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
#else
    public static bool SeqEqual<T>(T[]? left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool SeqEqual<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }

    public static bool SeqEqual<T>(IReadOnlyList<T> left, IReadOnlyList<T> right)
    {
        if (right.Count != left.Count)
            return false;

        for (var i = 0; i < left.Count; i++)
        {
            if (!FastEqual(left[i], right[i]))
                return false;
        }

        return true;
    }

#endif

#region Text
    public static bool TextEqual(string? left, string? right)
    {
        return string.Equals(left, right);
    }

    public static bool TextEqual(string? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }

    public static bool TextEqual(string? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }

    public static bool TextEqual(ReadOnlySpan<char> left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }

    public static bool TextEqual(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool TextEqual(ReadOnlySpan<char> left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool TextEqual(char[]? left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }

    public static bool TextEqual(char[]? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool TextEqual(char[]? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }


    public static bool TextEqual(string? left, string? right, StringComparison comparison)
    {
        return string.Equals(left, right, comparison);
    }

    public static bool TextEqual(string? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }

    public static bool TextEqual(string? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }

    public static bool TextEqual(ReadOnlySpan<char> left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }

    public static bool TextEqual(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool TextEqual(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool TextEqual(char[]? left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }

    public static bool TextEqual(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool TextEqual(char[]? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }
#endregion

    public static bool SetEqual<T>(ICollection<T>? left, ICollection<T>? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        if (left.Count != right.Count)
            return false;

        foreach (var item in left)
        {
            if (!right.Contains(item))
                return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TypeEqual(Type? left, Type? right) => left == right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TypeEqual<L, R>(L? left, R? right) => typeof(L) == typeof(R);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastEqual<T>(T? left, T? right)
    {
        return EqualityComparer<T>.Default.Equals(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FastEquality<T, U>(T? left, U? right)
        where T : IEquatable<U>
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }
}