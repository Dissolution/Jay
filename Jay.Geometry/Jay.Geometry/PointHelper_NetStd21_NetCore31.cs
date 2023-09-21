#if !NET48 && !NETSTANDARD2_0
using Jay.Memory;
using Jay.Text.Extensions;

namespace Jay.Geometry;

public static partial class PointHelper
{
    public delegate bool TryParseNumberSpan<T>(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, [NotNullWhen(true)] out T? value);
    
    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        NumberStyles numberStyle,
        IFormatProvider? provider,
        TryParseNumberSpan<T> tryParseSpan,
        out (T X, T Y) point)
    {
        point = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '(')
            return false;

        var xChars = reader.TakeUntil(',');
        if (!tryParseSpan(xChars, numberStyle, provider, out var x))
            return false;

        reader.Skip();
        var yChars = reader.TakeUntil(')');
        if (!tryParseSpan(yChars, numberStyle, provider, out var y))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        point = (x, y);
        return true;
    }

    public delegate bool TryFormatNumber<in T>(T number, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default);
    
    public static bool TryFormat<T>(
        T x, T y, 
        Span<char> destination,
        TryFormatNumber<T> tryFormat,
        out int charsWritten,
        ReadOnlySpan<char> format = default, 
        IFormatProvider? provider = default)
    {
        charsWritten = 0;
        SpanWriter<char> writer = stackalloc char[destination.Length];
        if (!writer.TryWrite('('))
            return false;
        if (!tryFormat(x, writer.RemainingSpan, out var xWritten, format, provider))
            return false;

        writer.Position += xWritten;
        if (!writer.TryWrite(','))
            return false;
        if (!tryFormat(y, writer.RemainingSpan, out var yWritten, format, provider))
            return false;

        writer.Position += yWritten;
        if (!writer.TryWrite(')'))
            return false;

        writer.WrittenSpan.CopyTo(destination);
        charsWritten = writer.Position;
        return true;
    }
}
#endif