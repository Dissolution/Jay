using Jay.Collections;
using Jay.Comparison;

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
            if (selectWherePredicate(input, out var output))
            {
                yield return output;
            }
        }
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T> source, T? defaultValue = default)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return defaultValue;
        T value = e.Current;
        if (e.MoveNext()) return defaultValue;
        return value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T> source,
        Func<T, bool> predicate, T? defaultValue = default)
    {
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

    public static IEnumerable<T> Double<T>(this IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
            throw new ArgumentException("There are no elements", nameof(source));
        var firstValue = e.Current;
        if (!e.MoveNext())
            throw new ArgumentException("There is only one element", nameof(source));
        var secondValue = e.Current;
        if (e.MoveNext())
            throw new ArgumentException("There are more than two elements", nameof(source));
        yield return firstValue;
        yield return secondValue;
    }

    public static IEnumerable<T> IgnoreExceptions<T>(this IEnumerable<T> enumerable)
    {
        IEnumerator<T> enumerator;
        try
        {
            enumerator = enumerable.GetEnumerator();
        }
        catch (Exception)
        {
            yield break;
        }

        while (true)
        {
            // Move next
            try
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }
            }
            catch (Exception)
            {
                // ignore this, stop enumerating
                enumerator.Dispose();
                yield break;
            }

            // Yield current
            T current;
            try
            {
                current = enumerator.Current;
            }
            catch (Exception)
            {
                // ignore this, but continue enumerating
                continue;
            }

            yield return current;
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(value => value is not null)!;
    }

    public static IEnumerable<EnumeratorItem<T>> Indexed<T>(this IEnumerable<T>? enumerable)
    {
        switch (enumerable)
        {
            case null:
                break;
            //IList<T>
            case IList<T> list:
            {
                var count = list.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < list.Count; i++)
                {
                    yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, list[i]);
                }
                break;
            }
            //ICollection<T>
            case ICollection<T> collection:
            {
                var count = collection.Count;
                if (count == 0)
                    yield break;
                int last = count - 1;
                using var e = collection.GetEnumerator();
                var i = 0;
                while (e.MoveNext())
                {
                    yield return new EnumeratorItem<T>(index: i,
                        sourceLength:
                        count,
                        isFirst: i == 0,
                        isLast: i == last,
                        value: e.Current);
                    i++;
                }
                break;
            }
            //IReadOnlyList<T>
            case IReadOnlyList<T> roList:
            {
                var count = roList.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < roList.Count; i++)
                {
                    yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, roList[i]);
                }
                break;
            }
            //IReadOnlyCollection<T>
            case IReadOnlyCollection<T> roCollection:
            {
                var count = roCollection.Count;
                if (count == 0)
                    yield break;
                using (var e = roCollection.GetEnumerator())
                {
                    var i = 0;
                    while (e.MoveNext())
                    {
                        yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, e.Current);
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
                        var current = e.Current;
                        //Move next now to check for last
                        last = !e.MoveNext();
                        //Return our entry
                        yield return new EnumeratorItem<T>(i, null, i == 0, last, current);
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

        return enumerable.OrderBy(selectSub,
            new FuncComparer<TSub>((x, y) =>
            {
                if (x == null) return y == null ? 0 : -1;
                if (y == null) return 1;
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
            foreach (var item in enumerable)
            {
                perItem(item);
            }
        }
    }

#if NETSTANDARD2_0 || NETSTANDARD2_1
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