namespace Jay.Geometry;

public readonly struct Point<T> : 
    IEqualityOperators<Point<T>, Point<T>, bool>, 
    IAdditionOperators<Point<T>, Point<T>, Point<T>>, 
    ISubtractionOperators<Point<T>, Point<T>, Point<T>>,
    IEquatable<Point<T>>,
    ISpanParsable<Point<T>>, 
    ISpanFormattable
    where T : INumber<T>, IMinMaxValue<T>, ISpanParsable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Point<T> first, Point<T> second) => first.X == second.X && first.Y == second.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Point<T> first, Point<T> second) => first.X != second.X || first.Y != second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point<T> operator +(Point<T> first, Point<T> second) => new Point<T>(first.X + second.X, first.Y + second.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point<T> operator -(Point<T> first, Point<T> second) => new Point<T>(first.X - second.X, first.Y - second.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point<T> operator -(Point<T> point) => new Point<T>(-point.X, -point.Y);

    public static readonly Point<T> Empty = new();
    
    public readonly T X;
    public readonly T Y;

    public bool IsEmpty => X == T.Zero && Y == T.Zero;
    
    public Point(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    public void Deconstruct(out T x, out T y)
    {
        x = this.X;
        y = this.Y;
    }

    public static bool TryParse(string? text, IFormatProvider? provider, out Point<T> point)
    {
        return TryParse((ReadOnlySpan<char>)text, provider, out point);
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Point<T> point)
    {
        point = default;    // fast return

        // Find the starting '('
        int s = text.IndexOf('(');
        if (s < 0) return false;

        // Find the ','
        int i = text[s..].IndexOf(',');
        if (i < 0) return false;

        // Get the X
        var xText = text[s..i].Trim();
        // Must parse
        if (!T.TryParse(xText, provider, out var x))
            return false;

        // Find the ending ')'
        int e = text[i..].IndexOf(")");
        if (e < 0) return false;

        // Get the Y
        var yText = text[i..e].Trim();
        // Must parse
        if (!T.TryParse(yText, provider, out var y))
            return false;

        // We can build
        point = new Point<T>(x, y);
        return true;

    }

    public static Point<T> Parse(string? text, IFormatProvider? provider = null)
        => Parse((ReadOnlySpan<char>)text, provider);

    public static Point<T> Parse(ReadOnlySpan<char> text, IFormatProvider? provider = null)
    {
        if (TryParse(text, provider, out var point)) return point;
        throw new ArgumentException($"Cannot parse \"{text}\" to a Point<{typeof(T)}>", nameof(text));
    }

    public Point<T> Clone() => this;


    /// <inheritdoc />
    public bool Equals(Point<T> point)
    {
        return this.X == point.X && this.Y == point.Y;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Point<T> point) return Equals(point);
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }


    public bool TryFormat(Span<char> destination, out int charsWritten, 
        ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        charsWritten = 0;
        int avail = destination.Length;

        // Account for at least `(,)`
        if (avail < 3) 
            return false;
        Span<char> buffer = stackalloc char[avail];
        int b = 0;

        buffer[b++] = '(';
        if (!X.TryFormat(buffer[b..], out var xWritten, format, provider)) 
            return false;
        b += xWritten;

        //We have at least `,)` left
        if (b + 2 > avail)
            return false;
        buffer[b++] = ',';
        if (!Y.TryFormat(buffer[b..], out var yWritten, format, provider))
            return false;
        b += yWritten;

        // We have a trailing `)`
        if (b + 1 > avail)
            return false;
        buffer[b++] = ')';

        buffer[..b].CopyTo(destination);
        charsWritten = b;
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var stringHandler = new DefaultInterpolatedStringHandler();
        stringHandler.AppendLiteral("(");
        stringHandler.AppendFormatted<T>(X, format);
        stringHandler.AppendLiteral(",");
        stringHandler.AppendFormatted<T>(Y, format);
        stringHandler.AppendLiteral(")");
        return stringHandler.ToStringAndClear();
    }


    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X},{Y})";
    }
}

