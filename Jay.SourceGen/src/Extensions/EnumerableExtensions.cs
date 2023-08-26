namespace Jay.CodeGen.Extensions;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/>s
/// </summary>
public static class EnumerableExtensions
{
    public static T One<T>(this IEnumerable<T> enumerable)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext())
            throw new ReflectedException("There are no items");
        var one = e.Current;
        if (e.MoveNext())
            throw new ReflectedException("There are too many items");
        return one;
    }
    
    [return: NotNullIfNotNull(nameof(fallback))]
    public static T? OneOrDefault<T>(this IEnumerable<T> enumerable, T? fallback = default)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext()) return fallback; // none
        var one = e.Current;
        if (e.MoveNext()) return fallback;// more than one
        return one;
    }

    public static T OneOrThrow<T>(this IEnumerable<T> enumerable, Func<int, Exception> getException)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext())
            throw getException(0);
        var one = e.Current;
        if (e.MoveNext())
            throw getException(2);
        return one;
    }
    
    
    
    
    
    

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> predicate)
    {
        foreach (TIn input in source)
        {
            if (predicate(input, out var output))
                yield return output;
        }
    }
}

public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, out TOut output);