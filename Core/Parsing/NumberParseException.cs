using System.Globalization;

namespace Jay.Parsing;

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
        DestinationType = destinationType;
        InputText = new string(inputText);
        NumberStyle = numberStyle;
        FormatProvider = formatProvider;
    }

    public NumberParseException(Type destinationType,
        string? inputText,
        NumberStyles numberStyle,
        IFormatProvider? formatProvider,
        string? message = null,
       Exception? innerException = null)
       : base(GetMessage(destinationType, inputText, numberStyle, formatProvider, message), innerException)
    {
        DestinationType = destinationType;
        InputText = inputText;
        NumberStyle = numberStyle;
        FormatProvider = formatProvider;
    }
}
