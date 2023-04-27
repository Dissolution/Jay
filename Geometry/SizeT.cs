using System.Globalization;
using Jay.Parsing;

namespace Jay.Geometry;

public readonly struct Size<T> :
    IEqualityOperators<Size<T>, Size<T>, bool>,
    IAdditionOperators<Size<T>, Size<T>, Size<T>>,
    ISubtractionOperators<Size<T>, Size<T>, Size<T>>,
    IEquatable<Size<T>>,
    INumberParsable<Size<T>>, ISpanParsable<Size<T>>, IParsable<Size<T>>,
    ISpanFormattable, IFormattable,
    ICloneable<Size<T>>
    where T : INumber<T>, IMinMaxValue<T>, ISpanParsable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Size<T> first, Size<T> second) =>
        first.Width == second.Width && first.Height == second.Height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Size<T> first, Size<T> second) =>
        first.Width != second.Width || first.Height != second.Height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator +(Size<T> first, Size<T> second) =>
        new Size<T>(first.Width + second.Width, first.Height + second.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator -(Size<T> first, Size<T> second) =>
        new Size<T>(first.Width - second.Width, first.Height - second.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator -(Size<T> point) => new Size<T>(-point.Width, -point.Height);

    public static readonly Size<T> Empty = default;


    public readonly T Width;
    public readonly T Height;

    public bool HasArea => Width > T.Zero && Height > T.Zero;

    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    public void Deconstruct(out T width, out T height)
    {
        width = Width;
        height = Height;
    }

    public static Size<T> Parse(string? text, IFormatProvider? formatProvider = null)
        => Parse(text.AsSpan(), formatProvider);

    public static Size<T> Parse(ReadOnlySpan<char> text, IFormatProvider? formatProvider = null)
    {
        if (!TryParse(text, formatProvider, out var size)) 
            throw ParseException.Create<Size<T>>(text, formatProvider);
        return size;
    }
    
    public static bool TryParse(string? text, IFormatProvider? formatProvider, out Size<T> size) 
        => TryParse(text.AsSpan(), default, formatProvider, out size);

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? formatProvider, out Size<T> size)
        => TryParse(text, default, formatProvider, out size);
    
    public static bool TryParse(string? text, NumberStyles numberStyle, IFormatProvider? formatProvider, out Size<T> size)
        => TryParse(text.AsSpan(), numberStyle, formatProvider, out size);

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? formatProvider, out Size<T> size)
    {
        size = default; // fast return

        // Find the starting '['
        int s = text.IndexOf('[');
        if (s < 0) return false;

        // Find the 'x'
        int i = text[s..].IndexOf('x');
        if (i < 0) return false;

        // Get the Width
        var widthText = text[s..i].Trim();
        // Must parse
        if (!T.TryParse(widthText, numberStyle, formatProvider, out var width))
            return false;

        // Find the ending ']'
        int e = text[i..].IndexOf("]");
        if (e < 0) return false;

        // Get the Height
        var heightText = text[i..e].Trim();
        // Must parse
        if (!T.TryParse(heightText, numberStyle, formatProvider, out var height))
            return false;

        // We can build
        size = new Size<T>(width, height);
        return true;
    }

    public Size<T> Clone() => this;

    public Size<T> DeepClone() => this;

    /// <inheritdoc />
    public bool Equals(Size<T> size)
    {
        return Width == size.Width && Height == size.Height;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Size<T> size) return Equals(size);
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        charsWritten = 0;
        int avail = destination.Length;

        // Account for at least `[x]`
        if (avail < 3)
            return false;
        Span<char> buffer = stackalloc char[avail];
        int b = 0;

        buffer[b++] = '[';
        if (!Width.TryFormat(buffer[b..], out var widthWritten, format, provider))
            return false;
        b += widthWritten;

        //We have at least `x]` left
        if (b + 2 > avail)
            return false;
        buffer[b++] = 'x';
        if (!Height.TryFormat(buffer[b..], out var heightWritten, format, provider))
            return false;
        b += heightWritten;

        // We have a trailing `]`
        if (b + 1 > avail)
            return false;
        buffer[b++] = ']';

        buffer[..b].CopyTo(destination);
        charsWritten = b;
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var stringHandler = new DefaultInterpolatedStringHandler();
        stringHandler.AppendLiteral("[");
        stringHandler.AppendFormatted<T>(Width, format);
        stringHandler.AppendLiteral("x");
        stringHandler.AppendFormatted<T>(Height, format);
        stringHandler.AppendLiteral("]");
        return stringHandler.ToStringAndClear();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Width}x{Height}]";
    }
}