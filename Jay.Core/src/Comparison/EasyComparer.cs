using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Jay.Comparison;

public partial class EasyComparer
{
    private static readonly ConcurrentDictionary<Type, IEqualityComparer> _equalityComparerCache = new();

    public static EasyComparer Instance { get; } = new();
    
    public static IEqualityComparer<T> DefaultEqualityComparer<T>() => EqualityComparer<T>.Default;
    
    public static IEqualityComparer DefaultEqualityComparer(Type type)
    {
        return _equalityComparerCache.GetOrAdd(type,
            t => (IEqualityComparer)typeof(EqualityComparer<>)
                    .MakeGenericType(t)
                    .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                    .ThrowIfNull()
                    .GetValue(null)!);
    }
    
    private static readonly ConcurrentDictionary<Type, IComparer> _comparerCache = new();
    
    public static IComparer<T> DefaultComparer<T>() => Comparer<T>.Default;

    public static IComparer DefaultComparer(Type type)
    {
        return _comparerCache.GetOrAdd(type,
            t => typeof(Comparer<>)
                .MakeGenericType(t)
                .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                .ThrowIfNull()
                .GetValue(null)
                .AsValid<IComparer>());
    }
    
    public static int Compare(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is null) return -1;
        if (right is null) return 1;
        return DefaultComparer(left.GetType())
            .Compare(left, right);
    }
    
    public new static bool Equals(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return DefaultEqualityComparer(left.GetType())
            .Equals(left, right);
    }
    
    public static int GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        return DefaultEqualityComparer(obj.GetType()).GetHashCode(obj);
    }
}


public partial class EasyComparer : 
    IEqualityComparer<object?>, IEqualityComparer,
    IComparer<object?>, IComparer
{
    int IComparer<object?>.Compare(object? x, object? y) => Compare(x, y);
    int IComparer.Compare(object? x, object? y) => Compare(x, y);
    
    bool IEqualityComparer<object?>.Equals(object? x, object? y) => Equals(x, y);
    bool IEqualityComparer.Equals(object? x, object? y) => Equals(x, y);
    
    int IEqualityComparer<object?>.GetHashCode(object? obj)=> GetHashCode(obj);
    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj);
}