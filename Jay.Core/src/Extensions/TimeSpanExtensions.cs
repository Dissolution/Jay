namespace Jay.Extensions;

/// <summary>
/// Extensions for <see cref="TimeSpan" />
/// </summary>
public static class TimeSpanExtensions
{
#region Multiply
    /// <summary>
    /// Multiply this <see cref="TimeSpan" /> value by the specified amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static TimeSpan MultiplyBy(this TimeSpan timeSpan, double multiplier)
    {
#if !(NET48 || NETSTANDARD2_0)
        return timeSpan * multiplier;
#else
        long ticks = timeSpan.Ticks;
        double newTicks = ticks * multiplier;
        return TimeSpan.FromTicks((long)newTicks);
#endif
    }
    
    /// <summary>
    /// Multiply this <see cref="TimeSpan" /> value by the specified amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static TimeSpan MultiplyBy(this TimeSpan timeSpan, long multiplier)
    {
#if !(NET48 || NETSTANDARD2_0)
        return timeSpan * multiplier;
#else
        long ticks = timeSpan.Ticks;
        long newTicks = ticks * multiplier;
        return TimeSpan.FromTicks(newTicks);
#endif
    }
#endregion

#region Divide
    /// <summary>
    /// Multiply this <see cref="TimeSpan" /> value by the specified amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="divider"></param>
    /// <returns></returns>
    public static TimeSpan DivideBy(this TimeSpan timeSpan, double divider)
    {
        if (divider.Equals(0d))
            throw new ArgumentOutOfRangeException(nameof(divider));

#if !(NET48 || NETSTANDARD2_0)
        return timeSpan / divider;
#else
        long ticks = timeSpan.Ticks;
        double newTicks = ticks / divider;
        return TimeSpan.FromTicks((long)newTicks);
#endif
    }

    /// <summary>
    /// Multiply this <see cref="TimeSpan" /> value by the specified amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="divider"></param>
    /// <returns></returns>
    public static TimeSpan DivideBy(this TimeSpan timeSpan, long divider)
    {
        if (divider == 0L)
            throw new ArgumentOutOfRangeException(nameof(divider));

#if !(NET48 || NETSTANDARD2_0)
        return timeSpan / divider;
#else
        long ticks = timeSpan.Ticks;
        long newTicks = ticks / divider;
        return TimeSpan.FromTicks(newTicks);
#endif
    }
    
#endregion

#region Rounding
    /// <summary>
    /// Drop this <see cref="TimeSpan" /> to the earliest <see cref="TimeSpan" /> precision floor.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static TimeSpan Floor(this TimeSpan timeSpan, TimeSpan precision)
    {
        long delta = timeSpan.Ticks % precision.Ticks;
        return TimeSpan.FromTicks(timeSpan.Ticks - delta);
    }

    /// <summary>
    /// Round this <see cref="TimeSpan" /> to the specified <see cref="TimeSpan" /> precision.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan precision)
    {
        long delta = timeSpan.Ticks % precision.Ticks;
        bool shouldRoundUp = delta > precision.Ticks / 2L;
        long offset = shouldRoundUp ? precision.Ticks : 0L;
        return TimeSpan.FromTicks(timeSpan.Ticks + (offset - delta));
    }

    /// <summary>
    /// Raise this <see cref="TimeSpan" /> to the latest <see cref="TimeSpan" /> precision ceiling.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static TimeSpan Ceiling(this TimeSpan timeSpan, TimeSpan precision)
    {
        long delta = timeSpan.Ticks % precision.Ticks;
        return TimeSpan.FromTicks(timeSpan.Ticks + (precision.Ticks - delta));
    }
#endregion
}