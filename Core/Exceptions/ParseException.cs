using Jay.Text;

namespace Jay.Exceptions;

public class ParseException : Exception
{
    protected static string GetParseExceptionMessage(Type destinationType, ReadOnlySpan<char> inputText, IFormatProvider? provider,
        string? customMessage)
    {
        using var _ = StringBuilderPool.Shared.Borrow(out var builder);

        builder.Append("Cannot parse \"")
            .Append(inputText)
            .Append("\" to a ")
            .Append(destinationType);

        if (provider is not null)
        {
            builder.Append(" with custom provider '")
                .Append(provider)
                .Append("'");
        }

        if (!string.IsNullOrWhiteSpace(customMessage))
        {
            builder.Append(": ").Append(customMessage);
        }

        return builder.ToString();
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
        : this(GetParseExceptionMessage(destinationType, inputText.AsSpan(), formatProvider, message), innerException)
    {
        this.DestinationType = destinationType;
        this.InputText = inputText;
        this.FormatProvider = formatProvider;
    }
}