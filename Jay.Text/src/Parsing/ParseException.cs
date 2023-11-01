using Jay.Reflection;
using Jay.Text.Building;

namespace Jay.Text.Parsing;

public class ParseException : ArgumentException
{
    public static ParseException CreateFor<T>(
        ReadOnlySpan<char> input,
        string? additionalInfo = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
    {
        var msg = TextBuilder.New
            .Append($"Could not parse '{input}' to a {typeof(T).NameOf()} value")
            .If(additionalInfo is not null, tb => tb.Append(": ").Append(additionalInfo))
            .ToStringAndDispose();
        return new ParseException(msg, inputName, innerException);
    }
    
    public static ParseException CreateFor<T>(
        string? input,
        string? additionalInfo = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
    {
        var msg = TextBuilder.New
            .Append($"Could not parse '{input}' to a {typeof(T).NameOf()} value")
            .If(additionalInfo is not null, tb => tb.Append(": ").Append(additionalInfo))
            .ToStringAndDispose();
        return new ParseException(msg, inputName, innerException);
    }

    private ParseException(string? message, string? paramName, Exception? innerException) : 
        base(message, paramName, innerException)
    {
    }
}