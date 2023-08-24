// ReSharper disable InvokeAsExtensionMethod

using Jay.Utilities;
#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

namespace Jay.Comparison;

public sealed class EnumerableEqualityComparer<T> :
    IEqualityComparer<T[]>,
    IEqualityComparer<IEnumerable<T>>
{
    public static EnumerableEqualityComparer<T> Default { get; } = new();

    private readonly IEqualityComparer<T> _equalityComparer;

    public EnumerableEqualityComparer(IEqualityComparer<T>? equalityComparer = default)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals(T[]? x, T[]? y)
    {
        return x.AsSpan().SequenceEqual(y.AsSpan(), _equalityComparer);
    }

    public bool Equals(Span<T> x, T[]? y)
    {
        return x.SequenceEqual(y.AsSpan(), _equalityComparer);
    }

    public bool Equals(ReadOnlySpan<T> x, T[]? y)
    {
        return x.SequenceEqual(y.AsSpan(), _equalityComparer);
    }

    public bool Equals(T[]? x, Span<T> y)
    {
        return x.AsSpan().SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(Span<T> x, Span<T> y)
    {
        return x.SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(ReadOnlySpan<T> x, Span<T> y)
    {
        return x.SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(T[]? x, ReadOnlySpan<T> y)
    {
        return x.AsSpan().SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(Span<T> x, ReadOnlySpan<T> y)
    {
        return x.SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
    {
        return x.SequenceEqual(y, _equalityComparer);
    }

    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        var comparer = _equalityComparer;
        using var xe = x.GetEnumerator();
        using var ye = y.GetEnumerator();
        while (true)
        {
            var xm = xe.MoveNext();
            if (ye.MoveNext() != xm) return false;
            if (!xm) break;

            if (!comparer.Equals(xe.Current, ye.Current)) return false;
        }
        // Length and Items are equal
        return true;
    }


    public int GetHashCode(T[]? items)
    {
        return Hasher.Combine<T>(items);
    }

    public int GetHashCode(ReadOnlySpan<T> items)
    {
        return Hasher.Combine<T>(items);
    }

    public int GetHashCode(Span<T> items)
    {
        return Hasher.Combine<T>(items);
    }

    public int GetHashCode(IEnumerable<T>? items)
    {
        return Hasher.Combine<T>(items);
    }
}