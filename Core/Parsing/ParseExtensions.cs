using System.Globalization;
using System.Numerics;
using Jay.Utilities;

namespace Jay.Parsing;

public static class ParseExtensions
{
    public const NumberStyles DefaultNumberStyle = NumberStyles.None;
    public const DateTimeStyles DefaultDateTimeStyle = DateTimeStyles.None;
    public const TimeSpanStyles DefaultTimeSpanStyle = TimeSpanStyles.None;
    

    #region IParsable

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, [MaybeNullWhen(false)] out T value)
        where T : IParsable<T> 
        => T.TryParse(text, null, out value);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
        where T : IParsable<T> 
        => T.TryParse(text, formatProvider, out value);

    public static T Parse<T>([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider = null)
          where T : IParsable<T>
    {
        if (!T.TryParse(text, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    #endregion

    #region ISpanParsable

    public static bool TryParse<T>(this ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value)
       where T : ISpanParsable<T>
       => T.TryParse(text, null, out value);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
        where T : ISpanParsable<T>
        => T.TryParse(text, formatProvider, out value);

    public static T Parse<T>(this ReadOnlySpan<char> text, IFormatProvider? formatProvider = null)
         where T : ISpanParsable<T>
    {
        if (!T.TryParse(text, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    #endregion

    #region INumberBase

    public static bool TryParse<T>(this ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T number, 
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T> 
        => T.TryParse(text, DefaultNumberStyle, null, out number);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T number,
       Constraints.IsNumberBase<T> _ = default)
       where T : INumberBase<T>
       => T.TryParse(text, DefaultNumberStyle, formatProvider, out number);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T> 
        => T.TryParse(text, numberStyle, null, out value);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T> 
        => T.TryParse(text, numberStyle, formatProvider, out value);


    public static bool TryParse<T>([NotNullWhen(true)] this string? text, [MaybeNullWhen(false)] out T number,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T>
        => T.TryParse(text, DefaultNumberStyle, null, out number);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T number,
       Constraints.IsNumberBase<T> _ = default)
       where T : INumberBase<T>
       => T.TryParse(text, DefaultNumberStyle, formatProvider, out number);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T>
        => T.TryParse(text, numberStyle, null, out value);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T>
        => T.TryParse(text, numberStyle, formatProvider, out value);

    public static T Parse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle = DefaultNumberStyle, IFormatProvider? formatProvider = null)
        where T : INumberBase<T>
    {
        if (!T.TryParse(text, numberStyle, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    public static T Parse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle = DefaultNumberStyle, IFormatProvider? formatProvider = null)
        where T : INumberBase<T>
    {
        if (!T.TryParse(text, numberStyle, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    #endregion

    #region INumberParsable

    public static bool TryParse<T>(this ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T number,
       Constraints.IsNumberParsable<T> _ = default)
       where T : INumberParsable<T>
       => T.TryParse(text, DefaultNumberStyle, null, out number);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T number,
       Constraints.IsNumberParsable<T> _ = default)
       where T : INumberParsable<T>
       => T.TryParse(text, DefaultNumberStyle, formatProvider, out number);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberParsable<T> _ = default)
        where T : INumberParsable<T>
        => T.TryParse(text, numberStyle, null, out value);

    public static bool TryParse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberParsable<T> _ = default)
        where T : INumberParsable<T>
        => T.TryParse(text, numberStyle, formatProvider, out value);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, [MaybeNullWhen(false)] out T number,
      Constraints.IsNumberParsable<T> _ = default)
      where T : INumberParsable<T>
      => T.TryParse(text, DefaultNumberStyle, null, out number);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T number,
       Constraints.IsNumberParsable<T> _ = default)
       where T : INumberParsable<T>
       => T.TryParse(text, DefaultNumberStyle, formatProvider, out number);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberParsable<T> _ = default)
        where T : INumberParsable<T>
        => T.TryParse(text, numberStyle, null, out value);

    public static bool TryParse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberParsable<T> _ = default)
        where T : INumberParsable<T>
        => T.TryParse(text, numberStyle, formatProvider, out value);

    public static T Parse<T>(this ReadOnlySpan<char> text, NumberStyles numberStyle = DefaultNumberStyle, IFormatProvider? formatProvider = null,
           Constraints.IsNumberParsable<T> _ = default)
      where T : INumberParsable<T>
    {
        if (!T.TryParse(text, numberStyle, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    public static T Parse<T>([NotNullWhen(true)] this string? text, NumberStyles numberStyle = DefaultNumberStyle, IFormatProvider? formatProvider = null,
           Constraints.IsNumberParsable<T> _ = default)
        where T : INumberParsable<T>
    {
        if (!T.TryParse(text, numberStyle, formatProvider, out var value))
            throw ParseException.Create<T>(text, formatProvider);
        return value;
    }

    #endregion

    #region DateTime

    public static bool TryParse(this ReadOnlySpan<char> text, out DateTime dateTime)
        => DateTime.TryParse(text, null, DefaultDateTimeStyle, out dateTime);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, out DateTime dateTime)
        => DateTime.TryParse(text, null, dateTimeStyle, out dateTime);
    public static bool TryParse(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, out DateTime dateTime)
        => DateTime.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateTime);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateTime dateTime) 
        => DateTime.TryParse(text, formatProvider, dateTimeStyle, out dateTime);

    public static bool TryParse([NotNullWhen(true)] this string? text, out DateTime dateTime)
       => DateTime.TryParse(text, null, DefaultDateTimeStyle, out dateTime);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, out DateTime dateTime)
        => DateTime.TryParse(text, null, dateTimeStyle, out dateTime);
    public static bool TryParse([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, out DateTime dateTime)
        => DateTime.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateTime);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateTime dateTime)
        => DateTime.TryParse(text, formatProvider, dateTimeStyle, out dateTime);
    #endregion

    #region DateTimeOffset

    public static bool TryParse(this ReadOnlySpan<char> text, out DateTimeOffset dateTimeOffset)
       => DateTimeOffset.TryParse(text, null, DefaultDateTimeStyle, out dateTimeOffset);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, null, dateTimeStyle, out dateTimeOffset);
    public static bool TryParse(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateTimeOffset);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, formatProvider, dateTimeStyle, out dateTimeOffset);

    public static bool TryParse([NotNullWhen(true)] this string? text, out DateTimeOffset dateTimeOffset)
      => DateTimeOffset.TryParse(text, null, DefaultDateTimeStyle, out dateTimeOffset);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, null, dateTimeStyle, out dateTimeOffset);
    public static bool TryParse([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateTimeOffset);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateTimeOffset dateTimeOffset)
        => DateTimeOffset.TryParse(text, formatProvider, dateTimeStyle, out dateTimeOffset);

    #endregion

    #region TimeSpan

    public static bool TryParse(this ReadOnlySpan<char> text, out TimeSpan timeSpan)
     => TimeSpan.TryParse(text, null, out timeSpan);
    public static bool TryParse(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, out TimeSpan timeSpan)
     => TimeSpan.TryParse(text, formatProvider, out timeSpan);

    public static bool TryParse([NotNullWhen(true)] this string? text, out TimeSpan timeSpan)
     => TimeSpan.TryParse(text, null, out timeSpan);
    public static bool TryParse([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, out TimeSpan timeSpan)
     => TimeSpan.TryParse(text, formatProvider, out timeSpan);

    #endregion

    #region TimeOnly

    public static bool TryParse(this ReadOnlySpan<char> text, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, null, DefaultDateTimeStyle, out timeOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, null, dateTimeStyle, out timeOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, formatProvider, DefaultDateTimeStyle, out timeOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, formatProvider, dateTimeStyle, out timeOnly);


    public static bool TryParse([NotNullWhen(true)] this string? text, out TimeOnly timeOnly)
       => TimeOnly.TryParse(text, null, DefaultDateTimeStyle, out timeOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, null, dateTimeStyle, out timeOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, formatProvider, DefaultDateTimeStyle, out timeOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out TimeOnly timeOnly)
        => TimeOnly.TryParse(text, formatProvider, dateTimeStyle, out timeOnly);

    #endregion

    #region DateOnly

    public static bool TryParse(this ReadOnlySpan<char> text, out DateOnly dateOnly)
        => DateOnly.TryParse(text, null, DefaultDateTimeStyle, out dateOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, out DateOnly dateOnly)
        => DateOnly.TryParse(text, null, dateTimeStyle, out dateOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, IFormatProvider? formatProvider, out DateOnly dateOnly)
        => DateOnly.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateOnly);
    public static bool TryParse(this ReadOnlySpan<char> text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateOnly dateOnly)
        => DateOnly.TryParse(text, formatProvider, dateTimeStyle, out dateOnly);

    public static bool TryParse([NotNullWhen(true)] this string? text, out DateOnly dateOnly)
       => DateOnly.TryParse(text, null, DefaultDateTimeStyle, out dateOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, out DateOnly dateOnly)
        => DateOnly.TryParse(text, null, dateTimeStyle, out dateOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, IFormatProvider? formatProvider, out DateOnly dateOnly)
        => DateOnly.TryParse(text, formatProvider, DefaultDateTimeStyle, out dateOnly);
    public static bool TryParse([NotNullWhen(true)] this string? text, DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider, out DateOnly dateOnly)
        => DateOnly.TryParse(text, formatProvider, dateTimeStyle, out dateOnly);

    #endregion
}