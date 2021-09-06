using System;
using System.Globalization;
using System.Linq;
using Jay.Text;

namespace Jay
{
	/// <summary>
	/// Extensions for <see cref="TimeSpan"/>
	/// </summary>
	public static class TimeSpanExtensions
	{
		#region Multiply

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static TimeSpan MultiplyBy(this TimeSpan timeSpan, double multiplier)
		{
			var ticks = timeSpan.Ticks;
			var newTicks = ticks * multiplier;
			return TimeSpan.FromTicks((long)newTicks);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static TimeSpan? MultiplyBy(this TimeSpan? timeSpan, double multiplier)
		{
			if (timeSpan is null)
				return null;
			return MultiplyBy(timeSpan.Value, multiplier);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static TimeSpan MultiplyBy(this TimeSpan timeSpan, long multiplier)
		{
			var ticks = timeSpan.Ticks;
			var newTicks = ticks * multiplier;
			return TimeSpan.FromTicks(newTicks);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static TimeSpan? MultiplyBy(this TimeSpan? timeSpan, long multiplier)
		{
			if (timeSpan is null)
				return null;
			return MultiplyBy(timeSpan.Value, multiplier);
		}
		#endregion

		#region Divide
		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="divider"></param>
		/// <returns></returns>
		public static TimeSpan DivideBy(this TimeSpan timeSpan, double divider)
		{
			if (divider.Equals(0d))
				throw new ArgumentOutOfRangeException(nameof(divider));

			var ticks = timeSpan.Ticks;
			var newTicks = ticks / divider;
			return TimeSpan.FromTicks((long)newTicks);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="divider"></param>
		/// <returns></returns>
		public static TimeSpan? DivideBy(this TimeSpan? timeSpan, double divider)
		{
			if (timeSpan is null)
				return null;
			return DivideBy(timeSpan.Value, divider);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="divider"></param>
		/// <returns></returns>
		public static TimeSpan DivideBy(this TimeSpan timeSpan, long divider)
		{
			if (divider == 0L)
				throw new ArgumentOutOfRangeException(nameof(divider));

			var ticks = timeSpan.Ticks;
			var newTicks = ticks / divider;
			return TimeSpan.FromTicks(newTicks);
		}

		/// <summary>
		/// Multiply this <see cref="TimeSpan"/> value by the specified amount.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="divider"></param>
		/// <returns></returns>
		public static TimeSpan? DivideBy(this TimeSpan? timeSpan, long divider)
		{
			if (timeSpan is null)
				return null;
			return DivideBy(timeSpan.Value, divider);
		}
		#endregion
		
		/// <summary>
		/// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(this TimeSpan timeSpan, string? format)
		{
			return Format(timeSpan, format, CultureInfo.CurrentCulture);
		}

		private static readonly char[] InvalidTimeSpanFormatChars = new char[5]
		{
			':', ',', '.', '\\', '/'
		};
		
		/// <summary>
		/// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="format"></param>
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		public static string Format(this TimeSpan timeSpan, string? format, IFormatProvider? formatProvider)
		{
			if (string.IsNullOrEmpty(format))
				return timeSpan.ToString();
			if (formatProvider is null)
				formatProvider = CultureInfo.CurrentCulture;

			/* TimeSpan formatting has gotchas.
			 * You have to escape certain characters for them to actually appear.
			 * Prepend backslash -->  '\'
			 */
			format = TextBuilder.Build(format, (tb, fmt) =>
			{
				if (string.IsNullOrWhiteSpace(fmt)) return;
				tb.EnsureCapacity(fmt.Length * 2);
				char c;
				for (var i = 0; i < fmt.Length; i++)
				{
					c = fmt[i];
					if (InvalidTimeSpanFormatChars.Contains(c))
						tb.Append('\\');
					tb.Append(c);
				}
			});

			return timeSpan.ToString(format, formatProvider);
		}

		/// <summary>
		/// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(this TimeSpan? timeSpan, string? format)
		{
			return Format(timeSpan, format, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="format"></param>
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		public static string Format(this TimeSpan? timeSpan, string? format, IFormatProvider? formatProvider)
		{
			return timeSpan is null ? string.Empty : Format(timeSpan.Value, format, formatProvider);
		}

		/// <summary>
		/// Get a SQL-compatible representation of this <see cref="TimeSpan"/>
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public static string ToSqlString(this TimeSpan timeSpan)
		{
			var format = @"hh\:mm\:ss";
			if (timeSpan.Days > 0)
				format = @"d\." + format;
			if (timeSpan.Milliseconds > 0)
				format = format + @"\.fff";
			return timeSpan.ToString(format);
		}
		/// <summary>
		/// Get a SQL-compatable representation of this <see cref="TimeSpan"/>
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public static string ToSqlString(this TimeSpan? timeSpan)
		{
			return timeSpan is null ? string.Empty : ToSqlString(timeSpan.Value);
		}
		
		#region Rounding
		/// <summary>
		/// Drop this <see cref="TimeSpan"/> to the earliest <see cref="TimeSpan"/> precision floor.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static TimeSpan Floor(this TimeSpan timeSpan, TimeSpan precision)
		{
			var delta = timeSpan.Ticks % precision.Ticks;
			return TimeSpan.FromTicks(timeSpan.Ticks - delta);
		}

		/// <summary>
		/// Round this <see cref="TimeSpan"/> to the specified <see cref="TimeSpan"/> precision.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan precision)
		{
			var delta = timeSpan.Ticks % precision.Ticks;
			var shouldRoundUp = delta > (precision.Ticks / 2L);
			var offset = shouldRoundUp ? precision.Ticks : 0L;
			return TimeSpan.FromTicks(timeSpan.Ticks + (offset - delta));
		}

		/// <summary>
		/// Raise this <see cref="TimeSpan"/> to the latest <see cref="TimeSpan"/> precision ceiling.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static TimeSpan Ceiling(this TimeSpan timeSpan, TimeSpan precision)
		{
			var delta = timeSpan.Ticks % precision.Ticks;
			return TimeSpan.FromTicks(timeSpan.Ticks + (precision.Ticks - delta));
		}

		#endregion

		
	}
}
