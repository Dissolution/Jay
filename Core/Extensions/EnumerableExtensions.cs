using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jay
{
    public static class EnumerableExtensions
    {
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
			return new DateTime((long) Math.Round(ave));
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
			return TimeSpan.FromTicks((long) Math.Round(ave));
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

		public delegate bool SelectPredicate<in TIn, TOut>(TIn value, [MaybeNullWhen(false)] out TOut result);

		public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source,
		                                                        SelectPredicate<TIn, TOut> selectWhere)
		{
			foreach (TIn value in source)
			{
				if (selectWhere(value, out TOut? result))
				{
					yield return result;
				}
			}
		}

		public static T? OneOrDefault<T>(this IEnumerable<T?> source, T? @default = default(T))
		{
			if (source is null) return @default;
			if (source is IList<T> list)
			{
				if (list.Count == 1)
				{
					return list[0];
				}
			}

			using (var e = source.GetEnumerator())
			{
				if (!e.MoveNext())
					return @default;
				var value = e.Current;
				if (e.MoveNext())
					return @default;
				return value;
			}
		}

		public static void Consume<T>(this IEnumerable<T>? source, Action<T>? forEach)
		{
			if (source is null) return;
			if (forEach is null) return;
			using (var e = source.GetEnumerator())
			{
				while (e.MoveNext())
				{
					forEach(e.Current);
				}
			}
		}
    }
}