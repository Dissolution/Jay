namespace Jay.Geometry;

/// <summary>
/// A 2D Rectangle in <typeparamref name="T"/> space
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of the Rectangle's Coordinates
/// </typeparam>
public readonly struct Rectangle<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Rectangle<T>, Rectangle<T>, bool>,
    IAdditionOperators<Rectangle<T>, Rectangle<T>, Rectangle<T>>,
    ISubtractionOperators<Rectangle<T>, Rectangle<T>, Rectangle<T>>,
    IUnaryNegationOperators<Rectangle<T>, Rectangle<T>>,
    ISpanParsable<Rectangle<T>>,
    IParsable<Rectangle<T>>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IEquatable<Rectangle<T>>,
    ICloneable<Rectangle<T>>
    where T :
#if NET7_0_OR_GREATER
    INumber<T>, IMinMaxValue<T>
#elif NET6_0_OR_GREATER
    IEquatable<T>, IComparable<T>, ISpanFormattable
#else
    IEquatable<T>, IComparable<T>, IFormattable
#endif
{
    public static bool operator ==(Rectangle<T> first, Rectangle<T> second) => first.Equals(second);
    public static bool operator !=(Rectangle<T> first, Rectangle<T> second) => !first.Equals(second);

#if NET7_0_OR_GREATER
    public static Rectangle<T> operator +(Rectangle<T> first, Rectangle<T> second)
        => FromLTWH(first.X + second.X, first.Y + second.Y, first.Width + second.Width, first.Height + second.Height);
    public static Rectangle<T> operator -(Rectangle<T> first, Rectangle<T> second)
        => FromLTWH(first.X - second.X, first.Y - second.Y, first.Width - second.Width, first.Height - second.Height);
    public static Rectangle<T> operator -(Rectangle<T> rectangle)
        => FromLTWH(-rectangle.X, -rectangle.Y, -rectangle.Width, -rectangle.Height);

#endif

    public static readonly Rectangle<T> Empty = default;

#if NET7_0_OR_GREATER
    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out Rectangle<T> rectangle)
    {
        if (str is null)
        {
            rectangle = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), provider, out rectangle);
    }
    
    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles numberStyle, IFormatProvider? provider, out Rectangle<T> rectangle)
    {
        if (str is null)
        {
            rectangle = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), numberStyle, provider, out rectangle);
    }

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, out Rectangle<T> rectangle)
        => TryParse(text, numberStyle, default, out rectangle);

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Rectangle<T> rectangle)
    {
        rectangle = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '(') return false;

        var xText = reader.TakeUntil(',');
        if (!T.TryParse(xText, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != ',') return false;

        var yText = reader.TakeUntil(')');
        if (!T.TryParse(yText, provider, out var y))
            return false;

        if (!reader.TryTake(3, out var chars) || chars != ")-[")
            return false;
        
        var widthText = reader.TakeUntil('×');
        if (!T.TryParse(widthText, provider, out var width))
            return false;

        if (!reader.TryTake(out ch) || ch != '×') return false;

        var heightText = reader.TakeUntil(')');
        if (!T.TryParse(heightText, provider, out var height))
            return false;

        if (!reader.TryTake(out ch) || ch != '×' || reader.UnreadCount != 0) 
            return false;

        rectangle = FromLTWH(x, y, width, height);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, out Rectangle<T> rectangle)
    {
        rectangle = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '(') return false;

        var xText = reader.TakeUntil(',');
        if (!T.TryParse(xText, numberStyle, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != ',') return false;

        var yText = reader.TakeUntil(')');
        if (!T.TryParse(yText, numberStyle, provider, out var y))
            return false;

        if (!reader.TryTake(3, out var chars) || chars != ")-[")
            return false;
        
        var widthText = reader.TakeUntil('×');
        if (!T.TryParse(widthText, numberStyle, provider, out var width))
            return false;

        if (!reader.TryTake(out ch) || ch != '×') return false;

        var heightText = reader.TakeUntil(')');
        if (!T.TryParse(heightText, numberStyle, provider, out var height))
            return false;

        if (!reader.TryTake(out ch) || ch != '×' || reader.UnreadCount != 0) 
            return false;

        rectangle = FromLTWH(x, y, width, height);
        return true;
    }

    public static Rectangle<T> Parse([AllowNull, NotNull] string? str, IFormatProvider? provider = default)
        => TryParse(str, provider, out var rectangle) ? rectangle : throw ParseException.CreateFor<T>(str);
    
    public static Rectangle<T> Parse([AllowNull, NotNull] string? str, NumberStyles numberStyles, IFormatProvider? provider = default)
        => TryParse(str, numberStyles, provider, out var rectangle) ? rectangle : throw ParseException.CreateFor<T>(str);

    public static Rectangle<T> Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
        => TryParse(text, provider, out var rectangle) ? rectangle : throw ParseException.CreateFor<T>(text);

#endif

#if NET7_0_OR_GREATER
    public static Rectangle<T> FromLTRB(T left, T top, T right, T bottom)
    {
        return new Rectangle<T>(left, top, left + right, top + bottom);
    }
#endif

    public static Rectangle<T> FromLTWH(T left, T top, T width, T height)
    {
        return new Rectangle<T>(left, top, width, height);
    }

    public static Rectangle<T> FromLocationSize(Point<T> location, Size<T> size)
    {
        return new Rectangle<T>(location.X, location.Y, size.Width, size.Height);
    }

#if NET7_0_OR_GREATER
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
#endif

    public readonly T X;
    public readonly T Y;
    public readonly T Width;
    public readonly T Height;

    public T Left => X;
    public T Top => Y;
#if NET7_0_OR_GREATER
    public T Right => Left + Width;
    public T Bottom => Top + Height;
#endif

    public Point<T> Location => new Point<T>(Left, Top);

    public Size<T> Size => new Size<T>(Width, Height);

    public bool IsEmpty
    {
        get
        {
#if NET7_0_OR_GREATER
            return X == T.Zero && Y == T.Zero && Width == T.Zero && Height == T.Zero;
#else
            return EqualityComparer<T>.Default.Equals(this.X, default!)
                && EqualityComparer<T>.Default.Equals(this.Y, default!)
                && EqualityComparer<T>.Default.Equals(this.Width, default!)
                && EqualityComparer<T>.Default.Equals(this.Height, default!);
#endif
        }
    }

    private Rectangle(T left, T top, T width, T height)
    {
        this.X = left;
        this.Y = top;
        this.Width = width;
        this.Height = height;
    }

    public void Deconstruct(out T x, out T y, out T width, out T height)
    {
        x = this.X;
        y = this.Y;
        width = this.Width;
        height = this.Height;
    }

    object ICloneable.Clone() => Clone();
    public Rectangle<T> Clone() => new(X, Y, Width, Height);

#if NET7_0_OR_GREATER
    public bool Contains(Point<T> point)
    {
        (T x, T y) = point;
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }
#endif

    public bool Equals(Rectangle<T> rectangle)
    {
#if NET7_0_OR_GREATER
        return this.X == rectangle.X 
            && this.Y == rectangle.Y 
            && this.Width == rectangle.Width 
            && this.Height == rectangle.Height;
#else
        return EqualityComparer<T>.Default.Equals(this.X, rectangle.X)
            && EqualityComparer<T>.Default.Equals(this.Y, rectangle.Y)
            && EqualityComparer<T>.Default.Equals(this.Width, rectangle.Width)
            && EqualityComparer<T>.Default.Equals(this.Height, rectangle.Height);
#endif
    }

    public override bool Equals(object? obj)
    {
        return obj is Rectangle<T> rectangle && Equals(rectangle);
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(X, Y, Width, Height);
    }

    public override string ToString()
    {
        return $"({X},{Y})-[{Width}×{Height}]";
    }

    public string ToString(string? format, IFormatProvider? provider = default)
    {
        var text = new InterpolatedText(7, 4);
        text.Write('(');
        text.Write<T>(X, format, provider);
        text.Write(',');
        text.Write<T>(Y, format, provider);
        text.Write(")-[");
        text.Write<T>(Width, format, provider);
        text.Write('×');
        text.Write<T>(Height, format, provider);
        text.Write(']');
        return text.ToStringAndDispose();
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
        if (!writer.TryWrite(")-[")) return false;
        if (!writer.TryWrite(Width, format, provider)) return false;
        if (!writer.TryWrite('×')) return false;
        if (!writer.TryWrite(Height, format, provider)) return false;
        if (!writer.TryWrite(']')) return false;

        var written = writer.WrittenItems;
        written.CopyTo(destination);
        charsWritten = written.Length;
        return true;
    }
#endif
}