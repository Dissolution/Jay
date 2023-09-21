
using Jay.Memory;
using Jay.Text.Extensions;

namespace Jay.Geometry;

public static partial class PointHelper
{
    public delegate bool TryParseNumber<T>(string? str, NumberStyles numberStyle, IFormatProvider? provider, [NotNullWhen(true)] out T? value);
    
    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        NumberStyles numberStyle,
        IFormatProvider? provider,
        TryParseNumber<T> tryParse,
        out (T X, T Y) point)
    {
        point = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '(')
            return false;

        var xChars = reader.TakeUntil(',');
        if (!tryParse(xChars.ToString(), numberStyle, provider, out var x))
            return false;

        reader.Skip();
        var yChars = reader.TakeUntil(')');
        if (!tryParse(yChars.ToString(), numberStyle, provider, out var y))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        point = (x, y);
        return true;
    }
}