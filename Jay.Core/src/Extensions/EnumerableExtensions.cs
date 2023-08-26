using Jay.Collections;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Jay.Extensions;

public static class EnumerableExtensions
{
#if NETSTANDARD2_0
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        return new HashSet<T>(source);
    }
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? itemComparer)
    {
        return new HashSet<T>(source, itemComparer);
    }
#endif

    public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, out TOut output);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> selectWherePredicate)
    {
        foreach (TIn input in source)
        {
            if (selectWherePredicate(input, out TOut? output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Option<TOut>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Result<TOut>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

    public static T One<T>(this IEnumerable<T> enumerable)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext())
            throw new InvalidOperationException("There are no items");
        var one = e.Current;
        if (e.MoveNext())
            throw new InvalidOperationException("There are too many items");
        return one;
    }
    
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T>? source, T? defaultValue = default)
    {
        if (source is null) return defaultValue;
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return defaultValue;
        T value = e.Current;
        if (e.MoveNext()) return defaultValue;
        return value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T>? source,
        Func<T, bool> predicate, T? defaultValue = default)
    {
        if (source is null) return defaultValue;
        using var e = source.GetEnumerator();
        while (e.MoveNext())
        {
            T result = e.Current;
            if (predicate(result))
            {
                while (e.MoveNext())
                {
                    // If there is more than one match, fail
                    if (predicate(e.Current))
                    {
                        return defaultValue;
                    }
                }

                // Only one match
                return result;
            }
        }

        // No matches
        return defaultValue;
    }

    /// <summary>
    /// Enumerates exactly two elements from <paramref name="source"/>
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IEnumerable<T> Double<T>(this IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
            throw new ArgumentException("There are no elements", nameof(source));
        T firstValue = e.Current;
        if (!e.MoveNext())
            throw new ArgumentException("There is only one element", nameof(source));
        T secondValue = e.Current;
        if (e.MoveNext())
            throw new ArgumentException("There are more than two elements", nameof(source));
        yield return firstValue;
        yield return secondValue;
    }

   

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(value => value is not null)!;
    }

    /// <summary>
    /// Index the contents of this <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<EnumeratorItem<T>> Indexed<T>(this IEnumerable<T>? enumerable)
    {
        switch (enumerable)
        {
            case null:
                break;
            //IList<T>
            case IList<T> list:
            {
                int count = list.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < list.Count; i++)
                {
                    yield return new(i, count, i == 0, i == count - 1, list[i]);
                }
                break;
            }
            //ICollection<T>
            case ICollection<T> collection:
            {
                int count = collection.Count;
                if (count == 0)
                    yield break;
                int last = count - 1;
                using var e = collection.GetEnumerator();
                var i = 0;
                while (e.MoveNext())
                {
                    yield return new(i,
                        count,
                        i == 0,
                        i == last,
                        e.Current);
                    i++;
                }
                break;
            }
            //IReadOnlyList<T>
            case IReadOnlyList<T> roList:
            {
                int count = roList.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < roList.Count; i++)
                {
                    yield return new(i, count, i == 0, i == count - 1, roList[i]);
                }
                break;
            }
            //IReadOnlyCollection<T>
            case IReadOnlyCollection<T> roCollection:
            {
                int count = roCollection.Count;
                if (count == 0)
                    yield break;
                using (var e = roCollection.GetEnumerator())
                {
                    var i = 0;
                    while (e.MoveNext())
                    {
                        yield return new(i, count, i == 0, i == count - 1, e.Current);
                        i++;
                    }
                }
                break;
            }
            //Have to enumerate
            default:
            {
                using (var e = enumerable.GetEnumerator())
                {
                    //If we cannot move, we are done
                    if (!e.MoveNext())
                        yield break;

                    //Defaults to first, not last, with index 0
                    var last = false;
                    var i = 0;
                    while (!last)
                    {
                        //Get the current value
                        T current = e.Current;
                        //Move next now to check for last
                        last = !e.MoveNext();
                        //Return our entry
                        yield return new(i, null, i == 0, last, current);
                        //increment index
                        i++;
                    }
                }
                break;
            }
        }
    }

    public static IEnumerable<T> OrderBy<T, TSub>(this IEnumerable<T> enumerable,
        Func<T, TSub> selectSub,
        params TSub[] order)
        where TSub : IEquatable<TSub>
    {

        int getIndex(TSub value)
        {
            for (var i = 0; i < order.Length; i++)
            {
                if (order[i].Equals(value)) return i;
            }
            return order.Length;
        }

        return enumerable
            .OrderBy(selectSub, Comparer<TSub>.Create((x, y) =>
            {
                if (x is null) return y is null ? 0 : -1;
                if (y is null) return 1;
                return getIndex(x).CompareTo(getIndex(y));
            }));
    }

    public static void Consume<T>(this IEnumerable<T> enumerable, Action<T> perItem)
    {
        if (enumerable is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perItem(list[i]);
            }
        }
        else
        {
            foreach (T item in enumerable)
            {
                perItem(item);
            }
        }
    }

#if !NET6_0_OR_GREATER
    public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> enumerable, out int count)
    {
        if (enumerable is ICollection<T> collection)
        {
            count = collection.Count;
            return true;
        }
        if (enumerable is IReadOnlyCollection<T> roCollection)
        {
            count = roCollection.Count;
            return true;
        }
        count = 0;
        return false;
    }
#endif
}