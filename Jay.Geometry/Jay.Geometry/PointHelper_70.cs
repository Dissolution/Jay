using Jay.Memory;
using Jay.Text.Extensions;

#if NET7_0_OR_GREATER

namespace Jay.Geometry;

public static partial class PointHelper
{
    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        out (T X, T Y) point)
        where T : ISpanParsable<T>
    {
        point = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '(')
            return false;

        var xText = reader.TakeUntil(',');
        if (!T.TryParse(xText, provider, out var x))
            return false;

        reader.Skip();
        var yText = reader.TakeUntil(')');
        if (!T.TryParse(yText, provider, out var y))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        point = (x, y);
        return true;
    }

    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        NumberStyles numberStyle,
        IFormatProvider? provider,
        out (T X, T Y) point)
        where T : INumberBase<T>
    {
        point = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '(')
            return false;

        var xChars = reader.TakeUntil(',');
        if (!T.TryParse(xChars, numberStyle, provider, out var x))
            return false;

        reader.Skip();
        var yChars = reader.TakeUntil(')');
        if (!T.TryParse(yChars, numberStyle, provider, out var y))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        point = (x, y);
        return true;
    }
}
#endif