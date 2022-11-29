using Jay.Reflection.Conversion.Formatting;
using Jay.Reflection.Conversion.Parsing;

namespace Jay.Reflection.Conversion.Exceptions;

public class ParsingException : ConversionException
{
    private static string GetMessage<TOut>(ReadOnlySpan<char> text,
                                           ParseOptions options,
                                           string? message = default)
    {
        var msg = new FormatterStringHandler(512, 0, FormatterCache.AssemblyCache);
        msg.AppendLiteral("Unable to parse '");
        msg.AppendFormatted(text);
        msg.AppendLiteral("' to a ");
        msg.AppendFormatted(typeof(TOut));
        msg.AppendLiteral(" value");
        if (options != default)
        {
            msg.AppendLiteral(" ");
            msg.AppendFormatted(options);
        }
        if (!string.IsNullOrWhiteSpace(message))
        {
            msg.AppendLiteral(": ");
            msg.AppendLiteral(message);
        }
        return msg.ToStringAndClear();
    }

    public static ParsingException Create<TOut>(ReadOnlySpan<char> text,
                                            ParseOptions options,
                                            string? message = null,
                                            Exception? innerException = null)
    {
        return new ParsingException(typeof(ReadOnlySpan<char>),
                                  typeof(TOut),
                                  GetMessage<TOut>(text, options, message),
                                  innerException);
    }

    public ParsingException(Type? inputType, Type? outputType, string? message = null, Exception? innerException = null)
        : base(inputType, outputType, message, innerException)
    {
    }
}