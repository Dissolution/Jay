using System.Globalization;

namespace Jay.Reflection.Parsing;

public sealed record class ParseOptions
{
    public static readonly ParseOptions Default = new ParseOptions();

    public IFormatProvider? FormatProvider { get; init; } = null;
    public NumberStyles NumberStyle { get; init; } = NumberStyles.None;
    public DateTimeStyles DateTimeStyle { get; init; } = DateTimeStyles.None;

    public ParseOptions() { }

    public ParseOptions(IFormatProvider? formatProvider)
    {
        FormatProvider = formatProvider;
    }

    public ParseOptions(NumberStyles numberStyle, IFormatProvider? formatProvider = null)
    {
        NumberStyle = numberStyle;
        FormatProvider = formatProvider;
    }

    public ParseOptions(DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider = null)
    {
        DateTimeStyle = dateTimeStyle;
        FormatProvider = formatProvider;
    }
}