using Jay.Text.Building;

namespace Jay.Text.Parsing;

public class ParseException : ArgumentException
{
    public static ParseException CreateFor<T>(
        ReadOnlySpan<char> input,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
    {
        var msg = TextBuilder.New
            .Append($"Could not parse '{input}' to a {typeof(T)} value")
            .If(info is not null, tb => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new ParseException(msg, inputName, innerException);
    }

    private ParseException(string? message, string? paramName, Exception? innerException) : 
        base(message, paramName, innerException)
    {
    }
}