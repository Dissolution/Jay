﻿namespace Jay.Geometry;

public readonly struct Rectangle<T> : IGeometry<Rectangle<T>>
    where T : INumber<T>, IMinMaxValue<T>, ISpanParsable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Rectangle<T> first, Rectangle<T> second) => first.Equals(second);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Rectangle<T> first, Rectangle<T> second) => !first.Equals(second);

    public static Rectangle<T> operator +(Rectangle<T> first, Rectangle<T> second)
    {
        return FromLTRB(first.Left + second.Left, first.Top + second.Top, first.Right + second.Right, first.Bottom + second.Bottom);
    }

    public static Rectangle<T> operator -(Rectangle<T> first, Rectangle<T> second)
    {
        return FromLTRB(first.Left - second.Left, first.Top - second.Top, first.Right - second.Right, first.Bottom - second.Bottom);
    }

    public static Rectangle<T> FromLTRB(T left, T top, T right, T bottom)
    {
        return new Rectangle<T>(left, top, left + right, top + bottom);
    }

    public static Rectangle<T> FromLTWH(T left, T top, T width, T height)
    {
        return new Rectangle<T>(left, top, width, height);
    }

    public static Rectangle<T> FromLocationSize(Point<T> location, Size<T> size)
    {
        return new Rectangle<T>(location.X, location.Y, size.Width, size.Height);
    }

    public static Rectangle<T> FromPoints(params Point<T>[] points) => FromPoints((IEnumerable<Point<T>>)points);

    public static Rectangle<T> FromPoints(IEnumerable<Point<T>> points)
    {
        var left = T.MinValue;
        var top = T.MaxValue;
        var right = T.MinValue;
        var bottom = T.MinValue;
        foreach ((T x, T y) in points)
        {
            if (x < left) left = x;
            if (x > right) right = x;
            if (y < top) top = y;
            if (y > bottom) bottom = y;
        }

        return FromLTRB(left, top, right, bottom);
    }

    public static Rectangle<T> FromRectangles(params Rectangle<T>[] rectangles) =>
        FromRectangles((IEnumerable<Rectangle<T>>)rectangles);

    public static Rectangle<T> FromRectangles(IEnumerable<Rectangle<T>> rectangles)
    {
        var left = T.MinValue;
        var top = T.MaxValue;
        var right = T.MinValue;
        var bottom = T.MinValue;
        foreach (var rect in rectangles)
        {
            if (rect.Left < left) left = rect.Left;
            if (rect.Top < top) top = rect.Top;
            if (rect.Right > right) right = rect.Right;
            if (rect.Bottom > bottom) bottom = rect.Bottom;
        }

        return FromLTRB(left, top, right, bottom);
    }

    public readonly T X;
    public readonly T Y;
    public readonly T Width;
    public readonly T Height;

    public T Left => X;
    public T Top => Y;
    public T Right => Left + Width;
    public T Bottom => Top + Height;

    public Point<T> Location => new Point<T>(Left, Top);
    public Size<T> Size => new Size<T>(Width, Height);

    public bool IsEmpty => X == default && Y == default && Width == default && Height == default;

    private Rectangle(T left, T top, T width, T height)
    {
        this.X = left;
        this.Y = top;
        this.Width = width;
        this.Height = height;
    }

    public void Deconstruct(out T x, out T y, out T width, out T height)
    {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Rectangle<T> rectangle) => TryParse(text, default, provider, out rectangle);

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, out Rectangle<T> rectangle)
    {
        rectangle = default; // fast return

        // Find the '/'
        int i = text.IndexOf('/');
        if (i < 0) return false;

        // Get the Location
        var locationText = text[..i].Trim();
        // Must parse
        if (!Point<T>.TryParse(locationText, numberStyle, provider, out var location))
            return false;

        // Get the Size
        var sizeText = text[i..].Trim();
        // Must parse
        if (!Size<T>.TryParse(sizeText, numberStyle, provider, out var size))
            return false;

        // We can build
        rectangle = FromLocationSize(location, size);
        return true;
    }

    public static bool TryParse([NotNullWhen(true)] string? text, NumberStyles numberStyle, IFormatProvider? provider,
        out Rectangle<T> rectangle)
        => TryParse(text.AsSpan(), numberStyle, provider, out rectangle);

    
    public Rectangle<T> Clone() => this;

    public bool Contains(Point<T> point)
    {
        (T x, T y) = point;
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    public bool Equals(Rectangle<T> rectangle)
    {
        return rectangle.Left == Left &&
            rectangle.Top == Top &&
            rectangle.Right == Right &&
            rectangle.Bottom == Bottom;
    }

    public override bool Equals(object? obj)
    {
        return obj is Rectangle<T> rectangle && Equals(rectangle);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        charsWritten = 0;
        int avail = destination.Length;

        // Account for at least `/`
        if (avail < 1)
            return false;
        Span<char> buffer = stackalloc char[avail];
        int b = 0;

        if (!X.TryFormat(buffer[b..], out var xWritten, format, provider))
            return false;
        b += xWritten;

        //We have at least `/` left
        if (b + 1 > avail)
            return false;
        buffer[b++] = '/';
        if (!Y.TryFormat(buffer[b..], out var yWritten, format, provider))
            return false;
        b += yWritten;

        buffer[..b].CopyTo(destination);
        charsWritten = b;
        return true;
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        var stringHandler = new DefaultInterpolatedStringHandler(1, 2, provider);
        stringHandler.AppendFormatted<Point<T>>(Location, format);
        stringHandler.AppendLiteral("/");
        stringHandler.AppendFormatted<Size<T>>(Size, format);
        return stringHandler.ToStringAndClear();
    }

    public override string ToString()
    {
        return $"{Location}/{Size}";
        // System.Drawing.Rectangle
        // $"{{X={X},Y={Y},Width={Width},Height={Height}}}"
    }
}