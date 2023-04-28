namespace Jay.Exceptions;

public class ParseException : Exception
{
    protected static string GetParseExceptionMessage(Type destinationType, ReadOnlySpan<char> inputText, IFormatProvider? provider,
        string? customMessage)
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

    public static ParseException Create<T>(string? inputText,
        IFormatProvider? provider = null,
        string? message = null,
        Exception? innerException = null)
    {
        return new ParseException(typeof(T), inputText, provider, message, innerException);
    }

    public Type? DestinationType { get; protected set; }
    public string? InputText { get; protected set; }
    public IFormatProvider? FormatProvider { get; protected set; }

    protected ParseException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public ParseException(Type destinationType,
        ReadOnlySpan<char> inputText,
        IFormatProvider? formatProvider,
        string? message = null,
        Exception? innerException = null)
        : this(GetParseExceptionMessage(destinationType, inputText, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = inputText.ToString();
        this.FormatProvider = formatProvider;
    }

    public ParseException(Type destinationType,
        string? inputText,
        IFormatProvider? formatProvider,
        string? message = null,
        Exception? innerException = null)
        : this(GetParseExceptionMessage(destinationType, inputText, formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = inputText;
        this.FormatProvider = formatProvider;
    }
}