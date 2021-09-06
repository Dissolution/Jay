using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
	/// <summary>
	/// Extensions for <see cref="IComparable{T}"/> and <see cref="IComparable"/> values.
	/// </summary>
	public static class ComparableExtensions
	{
		/// <summary>
		/// Limit this <see cref="IComparable{T}"/> value between a minimum and maximum value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to limit.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static T Limit<T>(this T value, T minimum, T maximum)
			where T : IComparable<T>
		{
			if (Comparer<T>.Default.Compare(value, minimum) < 0)
				return minimum;
			if (Comparer<T>.Default.Compare(value, maximum) > 0)
				return maximum;
			return value;
		}

		/// <summary>
		/// Limit this <see cref="IComparable{T}"/> value between a minimum and maximum value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to limit.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static T Limit<T>(this T value, T? minimum = null, T? maximum = null)
			where T : struct, IComparable<T>
		{
			if (minimum.HasValue && value.CompareTo(minimum.Value) < 0)
				return minimum.Value;
			if (maximum.HasValue && value.CompareTo(maximum.Value) > 0)
				return maximum.Value;
			return value;
		}

		/// <summary>
		/// Limit this <see cref="IComparable{T}"/> value between a minimum and maximum value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to limit.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static T? Limit<T>(this T? value, T? minimum = null, T? maximum = null)
			where T : struct, IComparable<T>
		{
			return value?.Limit(minimum, maximum) ?? value;
		}
		
		/// <summary>
		/// Is this <see cref="IComparable{T}"/> value between a minimum and maximum value?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to check.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static bool IsBetween<T>(this T value, T? minimum, T? maximum)
			where T : struct, IComparable<T>
		{
			if (minimum.HasValue && !(value.CompareTo(minimum.Value) >= 0))
				return false;
			if (maximum.HasValue && !(value.CompareTo(maximum.Value) <= 0))
				return false;
			return true;
		}

		/// <summary>
		/// Is this <see cref="IComparable{T}"/> value between a minimum and maximum value?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to check.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static bool IsBetween<T>(this T? value, T? minimum, T? maximum)
			where T : struct, IComparable<T>
		{
			//If our value is null, we have to have a null minimum or maximum
			if (!value.HasValue)
				return !minimum.HasValue || !maximum.HasValue;
			return IsBetween(value.Value, minimum, maximum);
		}

		/// <summary>
		/// Is this <see cref="IComparable{T}"/> value between a minimum and maximum value?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The <see cref="IComparable{T}"/> value to check.</param>
		/// <param name="minimum">The minimum inclusive value it can be.</param>
		/// <param name="maximum">The maximum inclusive value it can be.</param>
		/// <returns></returns>
		public static bool IsBetween<T>(this T? value, T? minimum, T? maximum)
			where T : IComparable<T>
		{
			if (Comparer<T>.Default.Compare(value, minimum) < 0)
				return false;
			if (Comparer<T>.Default.Compare(value, maximum) > 0)
				return false;
			return true;
		}
	}
}
