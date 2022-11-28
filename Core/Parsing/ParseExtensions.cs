using Jay.Utilities;
using System.Globalization;
using System.Numerics;

namespace Jay;

public class NumberParseException : ParseException
{
    protected static string GetMessage(Type destinationType, ReadOnlySpan<char> inputText, NumberStyles numberStyle, IFormatProvider? provider, string? customMessage)
    {
        var builder = new DefaultInterpolatedStringHandler();
        builder.AppendLiteral("Cannot parse \"");
        builder.AppendFormatted(inputText);
        builder.AppendLiteral("\" to a ");
        builder.AppendFormatted<Type>(destinationType);
        {
            builder.AppendLiteral(" with ");
            builder.AppendFormatted<NumberStyles>(numberStyle);
        }
        if (provider is not null)
        {
            builder.AppendLiteral(" with custom provider '");
            builder.AppendFormatted<IFormatProvider>(provider);
            builder.AppendLiteral("'");
        }
        if (!string.IsNullOrWhiteSpace(customMessage))
        {
            builder.AppendLiteral(": ");
            builder.AppendLiteral(customMessage);
        }
        return builder.ToStringAndClear();
    }

    public static NumberParseException Create<T>(ReadOnlySpan<char> inputText,
        NumberStyles numberStyle,
       IFormatProvider? provider = null,
       string? message = null,
       Exception? innerException = null)
    {
        return new NumberParseException(typeof(T), inputText, numberStyle, provider, message, innerException);
    }

    public NumberStyles NumberStyle { get; protected set;} = default;

    public NumberParseException(Type destinationType,
       ReadOnlySpan<char> inputText,
       NumberStyles numberStyle,
       IFormatProvider? formatProvider,
       string? message = null,
      Exception? innerException = null)
      : base(GetMessage(destinationType, inputText, numberStyle, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = new string(inputText);
        this.NumberStyle = numberStyle;
        this.FormatProvider = formatProvider;
    }

    public NumberParseException(Type destinationType,
        string? inputText,
        NumberStyles numberStyle,
        IFormatProvider? formatProvider,
        string? message = null,
       Exception? innerException = null)
       : base(GetMessage(destinationType, inputText, numberStyle, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = inputText;
        this.NumberStyle = numberStyle;
        this.FormatProvider = formatProvider;
    }
}

public class ParseException : Exception
{
    protected static string GetMessage(Type destinationType, ReadOnlySpan<char> inputText, IFormatProvider? provider, string? customMessage)
    {
        var builder = new DefaultInterpolatedStringHandler();
        builder.AppendLiteral("Cannot parse \"");
        builder.AppendFormatted(inputText);
        builder.AppendLiteral("\" to a ");
        builder.AppendFormatted<Type>(destinationType);
        if (provider is not null)
        {
            builder.AppendLiteral(" with custom provider '");
            builder.AppendFormatted<IFormatProvider>(provider);
            builder.AppendLiteral("'");
        }
        if (!string.IsNullOrWhiteSpace(customMessage))
        {
            builder.AppendLiteral(": ");
            builder.AppendLiteral(customMessage);
        }
        return builder.ToStringAndClear();
    }

    public static ParseException Create<T>(ReadOnlySpan<char> inputText,
        IFormatProvider? provider = null,
        string? message = null,
        Exception? innerException = null)
    {
        return new ParseException(typeof(T), inputText, provider, message, innerException);
    }

    public string? InputText { get; protected set;} = null;
    public IFormatProvider? FormatProvider { get; protected set;} = null;
    public Type DestinationType { get; protected set;} = null!;

    protected ParseException(string message, Exception? innerException)
        : base(message, innerException)
    {

    }

    public ParseException(Type destinationType,
       ReadOnlySpan<char> inputText,
       IFormatProvider? formatProvider,
       string? message = null,
      Exception? innerException = null)
      : this(GetMessage(destinationType, inputText, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = new string(inputText);
        this.FormatProvider = formatProvider;
    }

    public ParseException(Type destinationType,
        string? inputText,
        IFormatProvider? formatProvider,
        string? message = null,
       Exception? innerException = null)
       : this(GetMessage(destinationType, inputText, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = inputText;
        this.FormatProvider = formatProvider;
    }
}

public static class ParseExtensions
{
    public static T Parse<T>(this ReadOnlySpan<char> text,
        IFormatProvider? provider)
                where T : ISpanParsable<T>
    {
        if (T.TryParse(text, provider, out var value))
            return value;
        throw ParseException.Create<T>(text, provider);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> text, 
        [MaybeNullWhen(false)] out T value)
        where T : ISpanParsable<T>
    {
        return T.TryParse(text, null, out value);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> text, 
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out T value)
        where T : ISpanParsable<T>
    {
        return T.TryParse(text, provider, out value);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> text, 
        [MaybeNullWhen(false)] out T number, 
        Constraints.IsNumberBase<T> _ = default)
       where T : INumberBase<T>
    {
        return T.TryParse(text, NumberStyles.Any, null, out number);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> text,
        NumberStyles numberStyle,
        [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T>
    {
        return T.TryParse(text, numberStyle, null, out value);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> text,
      NumberStyles numberStyle,
      IFormatProvider? provider,
      [MaybeNullWhen(false)] out T value,
      Constraints.IsNumberBase<T> _ = default)
      where T : INumberBase<T>
    {
        return T.TryParse(text, numberStyle, provider, out value);
    }


    public static bool TryParse<T>([NotNullWhen(true)] this string? text,
      [MaybeNullWhen(false)] out T value)
      where T : IParsable<T>
    {
        return T.TryParse(text, null, out value);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string? text,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out T value)
        where T : IParsable<T>
    {
        return T.TryParse(text, provider, out value);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string? text,
        [MaybeNullWhen(false)] out T number,
        Constraints.IsNumberBase<T> _ = default)
       where T : INumberBase<T>
    {
        return T.TryParse(text, NumberStyles.Any, null, out number);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string? text,
        NumberStyles numberStyle,
        [MaybeNullWhen(false)] out T value,
        Constraints.IsNumberBase<T> _ = default)
        where T : INumberBase<T>
    {
        return T.TryParse(text, numberStyle, null, out value);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string? text,
      NumberStyles numberStyle,
      IFormatProvider? provider,
      [MaybeNullWhen(false)] out T value,
      Constraints.IsNumberBase<T> _ = default)
      where T : INumberBase<T>
    {
        return T.TryParse(text, numberStyle, provider, out value);
    }
}