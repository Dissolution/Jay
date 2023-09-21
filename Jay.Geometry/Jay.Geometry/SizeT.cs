namespace Jay.Geometry;

public readonly struct Size<T> : IGeometry<Size<T>>
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
    
    public readonly T Width;
    public readonly T Height;

    public bool HasArea => Width > T.Zero && Height > T.Zero;

    public bool IsEmpty => Width == T.Zero && Height == T.Zero;

    public Size(T width, T height)
    {
        this.Width = width;
        this.Height = height;
    }

    public void Deconstruct(out T width, out T height)
    {
        width = this.Width;
        height = this.Height;
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Size<T> size)
        => TryParse(text, default, provider, out size);

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, out Size<T> size)
    {
        if (Size.TryParse<T>(text, numberStyle, provider, out var wh))
        {
            size = new(wh.Width, wh.Height);
            return true;
        }
        size = default;
        return false;
    }

    public Size<T> Clone() => this;
    
    public bool Equals(Size<T> size)
    {
        return this.Width == size.Width && this.Height == size.Height;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Size<T> size && Equals(size);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
        => Size.TryFormat<T>(
            Width, Height, destination, out charsWritten,
            format, provider);

    public string ToString(string? format, IFormatProvider? provider = null) 
        => Size.ToString(Width, Height, format, provider);
    
    public override string ToString() => Size.ToString(Width, Height);
}