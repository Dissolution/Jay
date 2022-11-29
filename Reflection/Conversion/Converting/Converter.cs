namespace Jay.Reflection.Conversion.Converting;

public sealed record class ConvertOptions
{
    public required IFormatProvider? FormatProvider { get; init; }

}

public class Converter
{
    public static Result TryConvert<T>(ReadOnlySpan<char> text, ConvertOptions options, [NotNullWhen(true)] out T? value)
        where T : ISpanParsable<T>
    {
        return T.TryParse(text, options.FormatProvider, out value);
    }
}