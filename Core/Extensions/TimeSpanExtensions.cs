using System.Diagnostics;
using Jay.Extensions;
using Jay.Text;

namespace Jay.Extensions;

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
        return timeSpan.Value.MultiplyBy(multiplier);
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
        return timeSpan.Value.MultiplyBy(multiplier);
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
        return timeSpan.Value.DivideBy(divider);
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
        return timeSpan.Value.DivideBy(divider);
    }
    #endregion

    private static bool IsFormatChar(char ch)
    {
        return ch is '%' or 'h' or 'd' or 'm' or 's' or 'f' or 'F';
    }

   
    ///// <summary>
    ///// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
    ///// </summary>
    ///// <param name="timeSpan"></param>
    ///// <param name="format"></param>
    ///// <param name="formatProvider"></param>
    ///// <returns></returns>
    //public static string Format(this TimeSpan timeSpan,
    //                            ReadOnlySpan<char> format = default,
    //                            IFormatProvider? formatProvider = default)
    //{
    //    const char slash = '\\';
    //    const char quote = '\'';

    //    var formatLen = format.Length;

    //    if (formatLen == 0)
    //        return timeSpan.ToString();

    //    /* TimeSpan formatting has gotchas.
    //     * You have to escape certain characters for them to actually appear.
    //     */
    //    var text = new CharSpanWriter();
    //    int i = 0;
    //    char ch;
    //    while (i < formatLen)
    //    {
    //        ch = format[i];
    //        // Escape
    //        if (ch == slash)
    //        {
    //            // Write the escape and the following char
    //            text.Write(slash);
    //            i++;
    //            if (i < formatLen)
    //            {
    //                text.Write(format[i]);
    //            }
    //        }
    //        // Format specifier char
    //        else if (IsFormatChar(ch))
    //        {
    //            // This is fine
    //            text.Write(ch);
    //            i++;
    //        }
    //        // Literal string escape
    //        else if (ch == quote)
    //        {
    //            // Skip the leading '
    //            i++;
    //            var literal = format.TakeUntil(ref i, quote);
    //            text.Write(literal);
    //            // Skip the trailing '
    //            i++;
    //        }
    //        else
    //        {
    //            // This would normally cause a problem, but we can fix it!
    //            // Grab starting with this char
    //            var literal = format.TakeUntil(ref i, c => IsFormatChar(c) || c is slash or quote);
    //            Debug.Assert(literal.Length >= 1);
    //            if (literal.Length == 1)
    //            {
    //                // Single char escape
    //                text.Write(slash);
    //                text.Write(literal[0]);
    //            }
    //            else
    //            {
    //                // Literal escape
    //                text.Write(quote);
    //                    text.Write(literal);
    //                    text.Write(quote);
    //            }
    //            // Start processing with the char that stopped us
    //        }
    //    }

    //    return timeSpan.ToString(text.ToStringAndDispose(), formatProvider);
    //}

    ///// <summary>
    ///// Converts this <see cref="TimeSpan"/> to a string representation using the specified Format.
    ///// </summary>
    ///// <param name="timeSpan"></param>
    ///// <param name="format"></param>
    ///// <param name="formatProvider"></param>
    ///// <returns></returns>
    //public static string Format(this TimeSpan? timeSpan,
    //                            ReadOnlySpan<char> format = default,
    //                            IFormatProvider? formatProvider = default)
    //{
    //    return timeSpan is null ? "" : timeSpan.Value.Format(format, formatProvider);
    //}

    /// <summary>
    /// Get a SQL-compatable representation of this <see cref="TimeSpan"/>
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
    public static string? ToSqlString(this TimeSpan? timeSpan)
    {
        return timeSpan is null ? null : timeSpan.Value.ToSqlString();
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
        var shouldRoundUp = delta > precision.Ticks / 2L;
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