using System.Reflection;
using Jay.Collections;
using Jay.Utilities;

namespace Jay.Comparison;

public sealed class ObjectComparer : 
    IEqualityComparer<object?>, IEqualityComparer,
    IComparer<object?>, IComparer
{
    private static readonly ConcurrentTypeDictionary<IEqualityComparer> _equalityComparerCache = new();
    private static readonly ConcurrentTypeDictionary<IComparer> _comparerCache = new();

    public static ObjectComparer Instance { get; } = new();

    private static Type GreatestCommonType(Type first, Type second)
    {
        var firstBaseTypes = first.GetAllBaseTypes(true).Reverse().Skip(1).ToList();
        var secondBaseTypes = second.GetAllBaseTypes(true).Reverse().Skip(2).ToList();
        Type gct = typeof(object);
        foreach (var fbt in firstBaseTypes)
        {
            foreach (var sbt in secondBaseTypes)
            {
                if (fbt.IsAssignableFrom(sbt) ||
                    sbt.IsAssignableFrom(fbt))
                {
                    gct = sbt;
                }
            }
        }
        return gct;
    }
    
    
    public static IEqualityComparer GetEqualityComparer(Type type)
    {
        return _equalityComparerCache.GetOrAdd(type,
            t =>
            {
                var defaultEqualityComparer = typeof(EqualityComparer<>)
                    .MakeGenericType(t)
                    .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)?
                    .GetValue(null) as IEqualityComparer;
                if (defaultEqualityComparer is null)
                    throw new InvalidOperationException();
                return defaultEqualityComparer;
            });
    }
    public static IComparer GetComparer(Type type)
    {
        return _comparerCache.GetOrAdd(type,
            t =>
            {
                var defaultComparer = typeof(Comparer<>)
                    .MakeGenericType(t)
                    .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)?
                    .GetValue(null) as IComparer;
                if (defaultComparer is null)
                    throw new InvalidOperationException();
                return defaultComparer;
            });
    }
    
      
    public new static bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        var xType = x.GetType();
        var yType = y.GetType();
        var gct = GreatestCommonType(xType, yType);
        return GetEqualityComparer(gct).Equals(x, y);
    }

    public static int GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        return GetEqualityComparer(obj.GetType()).GetHashCode(obj);
    }

    public static int Compare(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        var xType = x.GetType();
        var yType = y.GetType();
        var gct = GreatestCommonType(xType, yType);
        return GetComparer(gct).Compare(x, y);
    }
  
    bool IEqualityComparer<object?>.Equals(object? x, object? y) => Equals(x, y);
    bool IEqualityComparer.Equals(object? x, object? y) => Equals(x, y);
    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj);
    int IEqualityComparer<object?>.GetHashCode(object? obj) => GetHashCode(obj);
    int IComparer.Compare(object? x, object? y) => Compare(x, y);
    int IComparer<object?>.Compare(object? x, object? y) => Compare(x, y);
}