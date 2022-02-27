using System.Reflection;
using Jay.Collections;
using Jay.Comparision;
using Jay.Validation;

namespace Jay.Reflection;

public static class ComparerCache
{
    public sealed class CacheComparer : IEqualityComparer, IComparer
    {
        /// <inheritdoc />
        public bool Equal(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            var xType = x.GetType();
            if (!y.GetType().Implements(xType)) return false;
            return GetEqualityComparer(xType).Equals(x, y);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y) => Equal(x, y);

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

    public static CacheComparer Default { get; } = new CacheComparer();
    
    static ComparerCache()
    {
        _equalityComparers = new();
        _comparers = new();
    }

    public static FuncEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode) => new FuncEqualityComparer<T>(equals, getHashCode);
    
    public static FuncComparer<T> CreateComparer<T>(Func<T?, T?, int> compare) => new FuncComparer<T>(compare);

    private static IEqualityComparer GetEqualityComparer(Type type)
    {
        return _equalityComparers.GetOrAdd(type, t => typeof(EqualityComparer<>).MakeGenericType(t)
                                                                                .GetProperty(nameof(EqualityComparer<byte>.Default),
                                                                                             BindingFlags.Public | BindingFlags.Static)
                                                                                .ThrowIfNull($"Cannot find EqualityComparer<{t}>.Default")
                                                                                .GetStaticValue<IEqualityComparer>()
                                                                                .ThrowIfNull($"Cannot cast EqualityComparer<{t}> to IEqualityComparer"));
    }

    private static IComparer GetComparer(Type type)
    {
        return _comparers.GetOrAdd(type, t => typeof(Comparer<>).MakeGenericType(t)
                                                                .GetProperty(nameof(Comparer<byte>.Default),
                                                                             BindingFlags.Public | BindingFlags.Static)
                                                                .ThrowIfNull($"Cannot find the Comparer<{t}>.Default property")
                                                                .GetStaticValue<IComparer>()
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
}