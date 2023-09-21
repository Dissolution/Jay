using Jay.Memory;

#if NET6_0_OR_GREATER

namespace Jay.Geometry;

public static partial class PointHelper
{
    public static bool TryFormat<T>(
        T x, T y,
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
        where T : ISpanFormattable
    {
        charsWritten = 0;
        SpanWriter<char> writer = stackalloc char[destination.Length];
        if (!writer.TryWrite('('))
            return false;
        if (!x.TryFormat(writer.RemainingSpan, out var xWritten, format, provider))
            return false;

        writer.Position += xWritten;
        if (!writer.TryWrite(','))
            return false;
        if (!y.TryFormat(writer.RemainingSpan, out var yWritten, format, provider))
            return false;

        writer.Position += yWritten;
        if (!writer.TryWrite(')'))
            return false;

        writer.WrittenSpan.CopyTo(destination);
        charsWritten = writer.Position;
        return true;
    }

    public static string ToString<T>(T x, T y, string? format, IFormatProvider? provider)
    {
        var text = new DefaultInterpolatedStringHandler(3, 2, provider);
        text.AppendLiteral("(");
        text.AppendFormatted<T>(x, format);
        text.AppendLiteral(",");
        text.AppendFormatted<T>(y, format);
        text.AppendLiteral(")");
        return text.ToStringAndClear();
    }
}
#endif