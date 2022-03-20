using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Conversion;

public class ParseException : ConversionException
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
   
    public static ParseException Create<TOut>(ReadOnlySpan<char> text,
                                            ParseOptions options,
                                            string? message = null,
                                            Exception? innerException = null)
    {
        return new ParseException(typeof(ReadOnlySpan<char>),
                                  typeof(TOut),
                                  GetMessage<TOut>(text, options, message),
                                  innerException);
    }
    
    public ParseException(Type? inputType, Type? outputType, string? message = null, Exception? innerException = null) 
        : base(inputType, outputType, message, innerException)
    {
    }
}