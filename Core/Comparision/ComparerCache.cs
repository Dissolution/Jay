using System.Reflection;
using Jay.Collections;
using Jay.Reflection;
using Jay.Validation;

namespace Jay.Comparision;

public static class ComparerCache
{
    private sealed class CacheLookupComparers : IEqualityComparer, IComparer
    {
        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            var xType = x.GetType();
            if (!y.GetType().Implements(xType)) return false;
            return GetEqualityComparer(xType).Equals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(object? obj)
        {
            if (obj is null) return 0;
            return GetEqualityComparer(obj.GetType()).GetHashCode(obj);
        }

        /// <inheritdoc />
        public int Compare(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            var xType = x.GetType();
            if (!y.GetType().Implements(xType)) return 0;
            return GetComparer(xType).Compare(x, y);
        }
    }

    private static readonly ConcurrentTypeDictionary<IEqualityComparer> _equalityComparers;
    private static readonly ConcurrentTypeDictionary<IComparer> _comparers;
    private static readonly CacheLookupComparers _cacheLookupComparers = new CacheLookupComparers();


    public static IEqualityComparer EqualityComparer => _cacheLookupComparers;
    public static IComparer Comparer => _cacheLookupComparers;

    static ComparerCache()
    {
        _equalityComparers = new();
        _comparers = new();
    }

    public static FuncEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode) 
        => new FuncEqualityComparer<T>(equals, getHashCode);

    public static FuncComparer<T> CreateComparer<T>(Func<T?, T?, int> compare) 
        => new FuncComparer<T>(compare);

    private static IEqualityComparer GetEqualityComparer(Type type)
    {
        return _equalityComparers.GetOrAdd(type, t => typeof(EqualityComparer<>).MakeGenericType(t)
            .GetProperty(nameof(EqualityComparer<byte>.Default),
                BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull($"Cannot find EqualityComparer<{t}>.Default")
            .GetValue<Types.Static, IEqualityComparer>(ref Types.Static.Instance)
            .ThrowIfNull($"Cannot cast EqualityComparer<{t}> to IEqualityComparer"));
    }

    private static IComparer GetComparer(Type type)
    {
        return _comparers.GetOrAdd(type, t => typeof(Comparer<>).MakeGenericType(t)
            .GetProperty(nameof(Comparer<byte>.Default),
                BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull($"Cannot find the Comparer<{t}>.Default property")
            .GetValue<Types.Static, IComparer>(ref Types.Static.Instance)
            .ThrowIfNull($"Cannot cast Comparer<{t}> to IComparer"));
    }

    public static bool Equals(Type type, object? x, object? y)
    {
        return GetEqualityComparer(type).Equals(x, y);
    }

    public static int GetHashCode(Type type, object? obj)
    {
        if (obj is null) return 0;
        return GetEqualityComparer(type).GetHashCode(obj);
    }

    public static int Compare(Type type, object? left, object? right)
    {
        return GetComparer(type).Compare(left, right);
    }

    public static bool Equals<T>(T? x, T? y)
    {
        return EqualityComparer<T>.Default.Equals(x, y);
    }

    public static int GetHashCode<T>(T? value)
    {
        if (value is null) return 0;
        return EqualityComparer<T>.Default.GetHashCode(value);
    }

    public static int Compare<T>(T? x, T? y)
    {
        return Comparer<T>.Default.Compare(x, y);
    }
}