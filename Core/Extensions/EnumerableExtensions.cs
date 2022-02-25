using Jay.Collections;
using Jay.Comparision;

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

    public static IEnumerable<EnumeratorItem<T>> Indexed<T>(this IEnumerable<T>? enumerable)
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
                yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, list[i]);
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
                    yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, e.Current);
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
                yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, roList[i]);
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
                    yield return new EnumeratorItem<T>(i, count, i == 0, i == count - 1, e.Current);
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
                    yield return new EnumeratorItem<T>(i, null, i == 0, last, current);
                    //increment index
                    i++;
                }
            }
        }
    }

    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source,
                                            IComparer<T> comparer) =>
            source.OrderBy(t => t, comparer);

	public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source,
                                                      IComparer<T> comparer) =>
			source.OrderByDescending(t => t, comparer);

	#region Sum
	#region DateTime
	/// <summary>
	/// Computes the sum of a sequence of <see cref="DateTime"/> values.
	/// </summary>
	/// <param name="enumerable">A sequence of values that are used to calculate a sum.</param>
	/// <returns>The sum of the values.</returns>
	public static DateTime Sum(this IEnumerable<DateTime> enumerable)
	{
		if (enumerable is null)
			throw new ArgumentNullException(nameof(enumerable));
		var accumulator = enumerable.Sum(dt => dt.Ticks);
		return new DateTime(accumulator);
	}

	/// <summary>
	/// Computes the sum of selected <see cref="DateTime"/> values from a <typeparam name="T"></typeparam> sequence.
	/// </summary>
	/// <param name="values"></param>
	/// <param name="selector"></param>
	/// <returns>The sum of the values.</returns>
	public static DateTime Sum<T>(this IEnumerable<T> values, Func<T, DateTime> selector)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		if (selector is null)
			throw new ArgumentNullException(nameof(values));
		return values.Select(selector).Sum();
	}
	#endregion
	#region TimeSpan

	/// <summary>
	/// Computes the sum of a sequence of <see cref="TimeSpan"/> values.
	/// </summary>
	/// <param name="enumerable">A sequence of values that are used to calculate a sum.</param>
	/// <returns>The sum of the values.</returns>
	public static TimeSpan Sum(this IEnumerable<TimeSpan> enumerable)
	{
		if (enumerable is null)
			throw new ArgumentNullException(nameof(enumerable));
		var accumulator = enumerable.Sum(dt => dt.Ticks);
		return TimeSpan.FromTicks(accumulator);
	}

	/// <summary>
	/// Computes the sum of selected <see cref="TimeSpan"/> values from a <typeparam name="T"></typeparam> sequence.
	/// </summary>
	/// <param name="values"></param>
	/// <param name="selector"></param>
	/// <returns>The sum of the values.</returns>
	public static TimeSpan Sum<T>(this IEnumerable<T> values, Func<T, TimeSpan> selector)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		if (selector is null)
			throw new ArgumentNullException(nameof(values));
		return values.Select(selector).Sum();
	}
	#endregion
	#endregion

	#region Average
	#region DateTime
	/// <summary>
	/// Computes the average of a sequence of <see cref="DateTime"/> values.
	/// </summary>
	/// <param name="values"></param>
	/// <returns></returns>
	public static DateTime Average(this IEnumerable<DateTime> values)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		var ave = values.Average(dt => dt.Ticks);
		return new DateTime((long)Math.Round(ave));
	}
	/// <summary>
	/// Computes the average of selected <see cref="DateTime"/> values selected from a sequence.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="values"></param>
	/// <param name="selector"></param>
	/// <returns></returns>
	public static DateTime Average<T>(this IEnumerable<T> values, Func<T, DateTime> selector)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		return values.Select(selector).Average();
	}
	#endregion
	#region TimeSpan
	/// <summary>
	/// Computes the average of a sequence of <see cref="TimeSpan"/> values.
	/// </summary>
	/// <param name="values"></param>
	/// <returns></returns>
	public static TimeSpan Average(this IEnumerable<TimeSpan> values)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		var ave = values.Average(dt => dt.Ticks);
		return TimeSpan.FromTicks((long)Math.Round(ave));
	}
	/// <summary>
	/// Computes the average of selected <see cref="TimeSpan"/> values selected from a sequence.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="values"></param>
	/// <param name="selector"></param>
	/// <returns></returns>
	public static TimeSpan Average<T>(this IEnumerable<T> values, Func<T, TimeSpan> selector)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		return values.Select(selector).Average();
	}
	#endregion
	#endregion

	public static IEnumerable<T> OrderBy<T, TSub>(this IEnumerable<T> enumerable,
	                                              Func<T, TSub> selectSub,
	                                              params TSub[] order)
		where TSub : IEquatable<TSub>
	{

		Func<TSub, int> getIndex = value =>
		{
			for (var i = 0; i < order.Length; i++)
			{
				if (order[i].Equals(value)) return i;
			}

			return order.Length;
		};

		return enumerable.OrderBy(selectSub,
		                          new FuncComparer<TSub>((x, y) =>
		                          {
			                          if (x == null) return y == null ? 0 : -1;
			                          if (y == null) return 1;
			                          return getIndex(x).CompareTo(getIndex(y));
		                          }));
	}
}