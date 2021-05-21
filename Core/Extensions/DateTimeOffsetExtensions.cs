using System;

namespace Jay
{
	/// <summary>
	/// Extensions for <see cref="DateTimeOffset"/>
	/// </summary>
	public static class DateTimeOffsetExtensions
	{
		/// <summary>
		/// How much time has elapsed since this <see cref="DateTimeOffset"/> occured?
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static TimeSpan ElapsedSince(this DateTimeOffset DateTimeOffset)
		{
			return DateTimeOffset.Now - DateTimeOffset;
		}

		/// <summary>
		/// How much time has elapsed since this <see cref="DateTimeOffset"/> occured?
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static TimeSpan? ElapsedSince(this DateTimeOffset? DateTimeOffset)
		{
			if (DateTimeOffset is null)
				return null;
			return ElapsedSince(DateTimeOffset.Value);
		}

		/// <summary>
		/// Convert the value of the current <see cref="DateTimeOffset"/> object to local time.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset? ToLocalTime(this DateTimeOffset? DateTimeOffset)
		{
			return DateTimeOffset?.ToLocalTime();
		}

		/// <summary>
		/// Convert the value of the current <see cref="DateTimeOffset"/> object to universal time.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset? ToUniversalTime(this DateTimeOffset? DateTimeOffset)
		{
			return DateTimeOffset?.ToUniversalTime();
		}

		#region XStart + XEnd
		/// <summary>
		/// Get the start of the day portion of the specified DateTimeOffset
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset DayStart(this DateTimeOffset DateTimeOffset)
		{
			return DateTimeOffset.Date;
		}

		/// <summary>
		/// Get the end of the day portion of the specified DateTimeOffset
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset DayEnd(this DateTimeOffset DateTimeOffset)
		{
			return DateTimeOffset.Date.AddDays(1).AddTicks(-1);
		}

		/// <summary>
		/// Gets the start of the week containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <param name="startOfWeek"></param>
		/// <returns></returns>
		public static DateTimeOffset WeekStart(this DateTimeOffset DateTimeOffset, DayOfWeek startOfWeek = DayOfWeek.Sunday)
		{
			var diff = DateTimeOffset.DayOfWeek - startOfWeek;
			if (diff < 0)
				diff += 7;
			return DateTimeOffset.AddDays(-1 * diff).Date;
		}

		/// <summary>
		/// Gets the start of the week containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <param name="endOfWeek"></param>
		/// <returns></returns>
		public static DateTimeOffset WeekEnd(this DateTimeOffset DateTimeOffset, DayOfWeek endOfWeek = DayOfWeek.Saturday)
		{
			var diff = endOfWeek - DateTimeOffset.DayOfWeek;
			return DateTimeOffset.AddDays(diff).Date.AddDays(1).AddTicks(-1);
		}

		/// <summary>
		/// Gets the start of the month containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset MonthStart(this DateTimeOffset DateTimeOffset)
		{
			return new DateTimeOffset(DateTimeOffset.Year, DateTimeOffset.Month, 1, 0, 0, 0, DateTimeOffset.Offset).Date;
		}

		/// <summary>
		/// Gets the end of the month containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset MonthEnd(this DateTimeOffset DateTimeOffset)
		{
			return MonthStart(DateTimeOffset).AddMonths(1).AddTicks(-1);
		}

		/// <summary>
		/// Gets the start of the year containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset YearStart(this DateTimeOffset DateTimeOffset)
		{
			return new DateTimeOffset(DateTimeOffset.Year, 1, 1, 0, 0, 0, DateTimeOffset.Offset).Date;
		}

		/// <summary>
		/// Gets the end of the year containing this DateTimeOffset.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <returns></returns>
		public static DateTimeOffset YearEnd(this DateTimeOffset DateTimeOffset)
		{
			return YearStart(DateTimeOffset).AddYears(1).AddTicks(-1);
		}
		#endregion

		#region Rounding
		/// <summary>
		/// Drop this <see cref="DateTimeOffset"/> to the earliest <see cref="TimeSpan"/> precision floor.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static DateTimeOffset Floor(this DateTimeOffset DateTimeOffset, TimeSpan precision)
		{
			var delta = DateTimeOffset.Ticks % precision.Ticks;
			return DateTimeOffset.AddTicks(-delta);
		}

		/// <summary>
		/// Round this <see cref="DateTimeOffset"/> to the specified <see cref="TimeSpan"/> precision.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static DateTimeOffset Round(this DateTimeOffset DateTimeOffset, TimeSpan precision)
		{
			var delta = DateTimeOffset.Ticks % precision.Ticks;
			var shouldRoundUp = delta > (precision.Ticks / 2L);
			var offset = shouldRoundUp ? precision.Ticks : 0L;
			return DateTimeOffset.AddTicks(offset - delta);
		}


		/// <summary>
		/// Raise this <see cref="DateTimeOffset"/> to the latest <see cref="TimeSpan"/> precision ceiling.
		/// </summary>
		/// <param name="DateTimeOffset"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static DateTimeOffset Ceiling(this DateTimeOffset DateTimeOffset, TimeSpan precision)
		{
			var delta = DateTimeOffset.Ticks % precision.Ticks;
			return DateTimeOffset.AddTicks(precision.Ticks - delta);
		}
		#endregion
	}
}
