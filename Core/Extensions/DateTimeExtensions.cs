namespace Jay.Extensions;

/// <summary>
/// Extensions for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// How much time has elapsed since this <see cref="DateTime"/> occured?
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static TimeSpan ElapsedSince(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return DateTime.UtcNow - dateTime;
        return DateTime.Now - dateTime;
    }

    /// <summary>
    /// How much time has elapsed since this <see cref="DateTime"/> occured?
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static TimeSpan? ElapsedSince(this DateTime? dateTime)
    {
        if (dateTime is null)
            return null;
        return dateTime.Value.ElapsedSince();
    }

    /// <summary>
    /// Convert the value of the current <see cref="DateTime"/> object to local time.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? ToLocalTime(this DateTime? dateTime)
    {
        return dateTime?.ToLocalTime();
    }

    /// <summary>
    /// Convert the value of the current <see cref="DateTime"/> object to universal time.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? ToUniversalTime(this DateTime? dateTime)
    {
        return dateTime?.ToUniversalTime();
    }

    /// <summary>
    /// Creates a new <see cref="DateTime"/> that is this <see cref="DateTime"/> as the specified <see cref="DateTimeKind"/>
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static DateTime AsKind(this DateTime dateTime, DateTimeKind kind)
    {
        return DateTime.SpecifyKind(dateTime, kind);
    }

    /// <summary>
    /// Creates a new <see cref="DateTime"/> that is this <see cref="DateTime"/> as the specified <see cref="DateTimeKind"/>
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static DateTime? AsKind(this DateTime? dateTime, DateTimeKind kind)
    {
        if (dateTime is null)
            return null;
        return dateTime.Value.AsKind(kind);
    }

    #region XStart + XEnd
    /// <summary>
    /// Get the start of the day portion of the specified DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime DayStart(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Get the end of the day portion of the specified DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime DayEnd(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="startOfWeek"></param>
    /// <returns></returns>
    public static DateTime WeekStart(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Sunday)
    {
        var diff = dateTime.DayOfWeek - startOfWeek;
        if (diff < 0)
            diff += 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the start of the week containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="endOfWeek"></param>
    /// <returns></returns>
    public static DateTime WeekEnd(this DateTime dateTime, DayOfWeek endOfWeek = DayOfWeek.Saturday)
    {
        var diff = endOfWeek - dateTime.DayOfWeek;
        return dateTime.AddDays(diff).Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the month containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime MonthStart(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1).Date;
    }

    /// <summary>
    /// Gets the end of the month containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime MonthEnd(this DateTime dateTime)
    {
        return dateTime.MonthStart().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the year containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime YearStart(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1).Date;
    }

    /// <summary>
    /// Gets the end of the year containing this DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime YearEnd(this DateTime dateTime)
    {
        return dateTime.YearStart().AddYears(1).AddTicks(-1);
    }
    #endregion

    #region Rounding
    /// <summary>
    /// Drop this <see cref="DateTime"/> to the earliest <see cref="TimeSpan"/> precision floor.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Floor(this DateTime dateTime, TimeSpan precision)
    {
        var delta = dateTime.Ticks % precision.Ticks;
        return dateTime.AddTicks(-delta);
    }

    /// <summary>
    /// Round this <see cref="DateTime"/> to the specified <see cref="TimeSpan"/> precision.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Round(this DateTime dateTime, TimeSpan precision)
    {
        var delta = dateTime.Ticks % precision.Ticks;
        var shouldRoundUp = delta > precision.Ticks / 2L;
        var offset = shouldRoundUp ? precision.Ticks : 0L;
        return dateTime.AddTicks(offset - delta);
    }

    /// <summary>
    /// Raise this <see cref="DateTime"/> to the latest <see cref="TimeSpan"/> precision ceiling.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static DateTime Ceiling(this DateTime dateTime, TimeSpan precision)
    {
        var delta = dateTime.Ticks % precision.Ticks;
        return dateTime.AddTicks(precision.Ticks - delta);
    }
    #endregion
}