using Jay.Memory;
using Jay.Text.Extensions;

namespace Jay.Geometry;

public static class Size
{
    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        out (T Width, T Height) size)
        where T : ISpanParsable<T>
    {
        size = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '[')
            return false;

        var widthText = reader.TakeUntil('×');
        if (!T.TryParse(widthText, provider, out var width))
            return false;

        reader.Skip();
        var heightText = reader.TakeUntil(']');
        if (!T.TryParse(heightText, provider, out var height))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        size = (width, height);
        return true;
    }

    public static bool TryParse<T>(
        ReadOnlySpan<char> text,
        NumberStyles numberStyle,
        IFormatProvider? provider,
        out (T Width, T Height) size)
        where T : INumberBase<T>
    {
        size = default; // fast return

        var reader = new SpanReader<char>(text);

        reader.SkipWhiteSpace();
        if (!reader.TryTake(out char ch) || ch != '[')
            return false;

        var widthText = reader.TakeUntil('×');
        if (!T.TryParse(widthText, numberStyle, provider, out var width))
            return false;

        reader.Skip();
        var heightText = reader.TakeUntil(']');
        if (!T.TryParse(heightText, numberStyle, provider, out var height))
            return false;

        reader.Skip();
        reader.SkipWhiteSpace();
        if (reader.Remaining.Length != 0)
            return false;

        size = (width, height);
        return true;
    }

    public static bool TryFormat<T>(
        T width, T height, Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
        where T : ISpanFormattable
    {
        charsWritten = 0;
        SpanWriter<char> writer = stackalloc char[destination.Length];
        if (!writer.TryWrite('['))
            return false;
        if (!width.TryFormat(writer.RemainingSpan, out var widthWritten, format, provider))
            return false;

        writer.Position += widthWritten;
        if (!writer.TryWrite('×'))
            return false;
        if (!height.TryFormat(writer.RemainingSpan, out var heightWritten, format, provider))
            return false;

        writer.Position += heightWritten;
        if (!writer.TryWrite(']'))
            return false;

        writer.WrittenSpan.CopyTo(destination);
        charsWritten = writer.Position;
        return true;
    }

    public static string ToString<T>(
        T width, T height,
        string? format,
        IFormatProvider? provider = default)
    {
        var text = new DefaultInterpolatedStringHandler(3, 2, provider);
        text.AppendFormatted('[');
        text.AppendFormatted<T>(width, format);
        text.AppendFormatted('×');
        text.AppendFormatted<T>(height, format);
        text.AppendFormatted(']');
        return text.ToStringAndClear();
    }

    public static string ToString<T>(T width, T height)
    {
        return $"[{width}×{height}]";
    }
}