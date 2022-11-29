using System.Globalization;

namespace Jay.Reflection.Conversion.Parsing;

public sealed record class ParseOptions
{
    public static readonly ParseOptions Default = new ParseOptions();

    public string? ExactFormat { get; init; } = null;
    public string[]? ExactFormats { get; init; } = null;
    public IFormatProvider? FormatProvider { get; init; } = null;
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;
    public NumberStyles NumberStyle { get; init; } = NumberStyles.None;
    public DateTimeStyles DateTimeStyle { get; init; } = DateTimeStyles.None;
    public TimeSpanStyles TimeSpanStyle { get; init; } = TimeSpanStyles.None;
    //public bool OnlyUseFirstChar { get; init; } = false;

    public ParseOptions() { }

    public ParseOptions(NumberStyles numberStyle, IFormatProvider? formatProvider = null)
    {
        this.NumberStyle = numberStyle;
        this.FormatProvider = formatProvider;
    }

    public ParseOptions(DateTimeStyles dateTimeStyle, IFormatProvider? formatProvider = null)
    {
        this.DateTimeStyle = dateTimeStyle;
        this.FormatProvider = formatProvider;
    }

    public ParseOptions(TimeSpanStyles timeSpanStyle, IFormatProvider? formatProvider = null)
    {
        this.TimeSpanStyle = timeSpanStyle;
        this.FormatProvider = formatProvider;
    }
}