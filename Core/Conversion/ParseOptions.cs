using System.Globalization;
using Jay.Text;

namespace Jay.Conversion;

public readonly record struct ParseOptions
{
    public readonly string[]? ExactFormats = null;
    public readonly IFormatProvider? FormatProvider = null;
    public readonly StringComparison Comparison = StringComparison.CurrentCulture;
    public readonly NumberStyles NumberStyles = NumberStyles.Any;
    public readonly DateTimeStyles DateTimeStyles = DateTimeStyles.None;
    public readonly TimeSpanStyles TimeSpanStyles = TimeSpanStyles.None;
    public readonly bool UseFirstChar = false;

    public ParseOptions() { }

    public bool HasExactFormat([NotNullWhen(true)] out string? exactFormat)
    {
        if (ExactFormats is not null && ExactFormats.Length == 1)
        {
            exactFormat = ExactFormats[0];
            return false;
        }
        exactFormat = null;
        return false;
    }
    
    public override string ToString()
    {
        using var text = new TextBuilder();
        text.Write('(');
        if (!ExactFormats.IsNullOrEmpty())
        {
            if (ExactFormats.Length == 1)
            {
                text.Append("format:\"")
                    .Append(ExactFormats[0])
                    .Write("\",");
            }
            else
            {
                text.Append("formats:[")
                    .AppendDelimit(",", ExactFormats, (tb, format) => tb.Append('"').Append(format).Append('"'))
                    .Write("],");
            }
        }

        if (FormatProvider is not null)
        {
            text.Append("provider:")
                .Append(FormatProvider)
                .Write(',');
        }

        if (Comparison != default)
        {
            text.Append("comparison:")
                .Append(Comparison)
                .Write(',');
        }

        if (NumberStyles != NumberStyles.Any)
        {
            text.Append("NumberStyle:")
                .Append(NumberStyles)
                .Write(',');
        }
        
        if (DateTimeStyles != DateTimeStyles.None)
        {
            text.Append("DateTimeStyle:")
                .Append(DateTimeStyles)
                .Write(',');
        }
        
        if (TimeSpanStyles != TimeSpanStyles.None)
        {
            text.Append("TimeSpanStyle:")
                .Append(TimeSpanStyles)
                .Write(',');
        }

        if (UseFirstChar)
        {
            text.Write("FirstChar,");
        }

        text.RemoveLast(',');
        text.Write(')');
        return text.ToString();
    }
}