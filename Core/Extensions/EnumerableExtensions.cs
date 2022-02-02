using System.Diagnostics.CodeAnalysis;
using Jay.Collections;

namespace Jay;

public static class EnumerableExtensions
{
    public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, [NotNullWhen(true)] out TOut output);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> selectWhere)
    {
        foreach (var element in source)
        {
            if (selectWhere(element, out var output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<EnumeratorValue<T>> Indexed<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null)
        {
            yield break;
        }
        //IList<T>
        else if (enumerable is IList<T> list)
        {
            var count = list.Count;
            //No items, exit immediately
            if (count == 0)
                yield break;
            //For each item, yield the entry
            for (var i = 0; i < list.Count; i++)
            {
                yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, list[i]);
            }
        }
        //ICollection<T>
        else if (enumerable is ICollection<T> collection)
        {
            var count = collection.Count;
            if (count == 0)
                yield break;
            using (var e = collection.GetEnumerator())
            {
                var i = 0;
                while (e.MoveNext())
                {
                    yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, e.Current);
                    i++;
                }
            }
        }
        //IReadOnlyList<T>
        else if (enumerable is IReadOnlyList<T> roList)
        {
            var count = roList.Count;
            //No items, exit immediately
            if (count == 0)
                yield break;
            //For each item, yield the entry
            for (var i = 0; i < roList.Count; i++)
            {
                yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, roList[i]);
            }
        }
        //IReadOnlyCollection<T>
        else if (enumerable is IReadOnlyCollection<T> roCollection)
        {
            var count = roCollection.Count;
            if (count == 0)
                yield break;
            using (var e = roCollection.GetEnumerator())
            {
                var i = 0;
                while (e.MoveNext())
                {
                    yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, e.Current);
                    i++;
                }
            }
        }
        //Have to enumerate
        else
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
                    yield return new EnumeratorValue<T>(i, null, i == 0, last, current);
                    //increment index
                    i++;
                }
            }
        }
    }
}