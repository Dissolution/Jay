namespace Jay.Geometry;

/// <summary>
/// A 2D Point in <typeparamref name="T"/> space
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of the <see cref="X"/> and <see cref="Y"/> coordinates
/// </typeparam>
public readonly struct Point<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Point<T>, Point<T>, bool>,
    IAdditionOperators<Point<T>, Point<T>, Point<T>>,
    ISubtractionOperators<Point<T>, Point<T>, Point<T>>,
    IUnaryNegationOperators<Point<T>, Point<T>>,
    ISpanParsable<Point<T>>,
    IParsable<Point<T>>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IEquatable<Point<T>>, 
    ICloneable<Point<T>>
    where T :
#if NET7_0_OR_GREATER
    INumberBase<T>
#elif NET6_0_OR_GREATER
    IEquatable<T>, ISpanFormattable
#else
    IEquatable<T>, IFormattable
#endif
{
    public static bool operator ==(Point<T> first, Point<T> last) => first.Equals(last);
    public static bool operator !=(Point<T> first, Point<T> last) => !first.Equals(last);

#if NET7_0_OR_GREATER
    public static Point<T> operator +(Point<T> first, Point<T> second) => new Point<T>(first.X + second.X, first.Y + second.Y);
    public static Point<T> operator -(Point<T> first, Point<T> second) => new Point<T>(first.X - second.X, first.Y - second.Y);
    public static Point<T> operator -(Point<T> point) => new Point<T>(-point.X, -point.Y);
#endif

    public static readonly Point<T> Empty = default;

#if NET7_0_OR_GREATER
    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out Point<T> point)
    {
        if (str is null)
        {
            point = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), provider, out point);
    }

    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles numberStyles, out Point<T> point)
        => TryParse(str, numberStyles, default, out point);
    
    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles numberStyles, IFormatProvider? provider, out Point<T> point)
    {
        if (str is null)
        {
            point = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), numberStyles, provider, out point);
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Point<T> point)
    {
        point = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '(') return false;

        var xText = reader.TakeUntil(',');
        if (!T.TryParse(xText, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != ',') return false;

        var yText = reader.TakeUntil(')');
        if (!T.TryParse(yText, provider, out var y))
            return false;

        if (!reader.TryTake(out ch) || ch != ')' || reader.UnreadCount != 0)
            return false;

        point = new(x, y);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, out Point<T> point)
        => TryParse(text, numberStyle, default, out point);
    
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, out Point<T> point)
    {
        point = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '(') return false;

        var xText = reader.TakeUntil(',');
        if (!T.TryParse(xText, numberStyle, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != ',') return false;

        var yText = reader.TakeUntil(')');
        if (!T.TryParse(yText, numberStyle, provider, out var y))
            return false;

        if (!reader.TryTake(out ch) || ch != ')' || reader.UnreadCount != 0)
            return false;

        point = new(x, y);
        return true;
    }

    public static Point<T> Parse([AllowNull, NotNull] string? str, IFormatProvider? provider = default)
        => TryParse(str, provider, out var point) ? point : throw ParseException.CreateFor<T>(str);
    
    public static Point<T> Parse([AllowNull, NotNull] string? str, NumberStyles numberStyles, IFormatProvider? provider = default)
        => TryParse(str, numberStyles, provider, out var point) ? point : throw ParseException.CreateFor<T>(str);

    public static Point<T> Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
        => TryParse(text, provider, out var point) ? point : throw ParseException.CreateFor<T>(text);
    
    public static Point<T> Parse(ReadOnlySpan<char> text, NumberStyles numberStyles, IFormatProvider? provider = default)
        => TryParse(text, numberStyles, provider, out var point) ? point : throw ParseException.CreateFor<T>(text);
#endif


    /// <summary>
    /// Gets the x-coordinate
    /// </summary>
    public readonly T X;

    /// <summary>
    /// Gets the y-coordinate
    /// </summary>
    public readonly T Y;

    public bool IsEmpty
    {
        get
        {
#if NET7_0_OR_GREATER
            return X == T.Zero && Y == T.Zero;
#else
            return EqualityComparer<T>.Default.Equals(this.X, default(T)!) 
                && EqualityComparer<T>.Default.Equals(this.Y, default(T)!);
#endif
        }
    }

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

    object ICloneable.Clone() => Clone();
    public Point<T> Clone() => new(X, Y);

    public bool Equals(Point<T> point)
    {
#if NET7_0_OR_GREATER
        return this.X == point.X && this.Y == point.Y;
#else
        return EqualityComparer<T>.Default.Equals(this.X, point.X) 
            && EqualityComparer<T>.Default.Equals(this.Y, point.Y);
#endif
    }

    public override bool Equals(object? obj)
    {
        return obj is Point<T> point && Equals(point);
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }

#if NET6_0_OR_GREATER
    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        charsWritten = 0; // fast return
        SpanWriter<char> writer = stackalloc char[destination.Length];
        
        if (!writer.TryWrite('(')) return false;
        if (!writer.TryWrite(X, format, provider)) return false;
        if (!writer.TryWrite(',')) return false;
        if (!writer.TryWrite(Y, format, provider)) return false;
        if (!writer.TryWrite(')')) return false;

        var written = writer.WrittenItems;
        written.CopyTo(destination);
        charsWritten = written.Length;
        return true;
    }
#endif
    
    public string ToString(string? format, IFormatProvider? provider = default)
    {
        var text = new InterpolatedText(3, 2);
        text.Write('(');
        text.Write<T>(X, format, provider);
        text.Write(',');
        text.Write<T>(Y, format, provider);
        text.Write(')');
        return text.ToStringAndDispose();
    }
}