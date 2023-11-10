namespace Jay.Geometry;

/// <summary>
/// A 2D Size in <typeparamref name="T"/> space
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of the <see cref="Width"/> and <see cref="Height"/>
/// </typeparam>
public readonly struct Size<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Size<T>, Size<T>, bool>,
    IAdditionOperators<Size<T>, Size<T>, Size<T>>,
    ISubtractionOperators<Size<T>, Size<T>, Size<T>>,
    IUnaryNegationOperators<Size<T>, Size<T>>,
    ISpanParsable<Size<T>>,
    IParsable<Size<T>>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IEquatable<Size<T>>
    where T :
#if NET7_0_OR_GREATER
    INumberBase<T>
#elif NET6_0_OR_GREATER
    IEquatable<T>, ISpanFormattable
#else
    IEquatable<T>, IFormattable
#endif
{
    public static bool operator ==(Size<T> first, Size<T> last) => first.Equals(last);
    public static bool operator !=(Size<T> first, Size<T> last) => !first.Equals(last);

#if NET7_0_OR_GREATER
    public static Size<T> operator +(Size<T> first, Size<T> second) => new Size<T>(first.Width + second.Width, first.Height + second.Height);
    public static Size<T> operator -(Size<T> first, Size<T> second) => new Size<T>(first.Width - second.Width, first.Height - second.Height);
    public static Size<T> operator -(Size<T> size) => new Size<T>(-size.Width, -size.Height);
#endif

    public static readonly Size<T> Empty = default;

#if NET7_0_OR_GREATER
    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out Size<T> size)
    {
        if (str is null)
        {
            size = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), provider, out size);
    }

    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles numberStyles, out Size<T> size)
        => TryParse(str, numberStyles, default, out size);
    
    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles numberStyles, IFormatProvider? provider, out Size<T> size)
    {
        if (str is null)
        {
            size = Empty;
            return false;
        }
        return TryParse(str.AsSpan(), numberStyles, provider, out size);
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Size<T> size)
    {
        size = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '[') return false;

        var xText = reader.TakeUntil('×');
        if (!T.TryParse(xText, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != '×') return false;

        var yText = reader.TakeUntil(']');
        if (!T.TryParse(yText, provider, out var y))
            return false;

        if (!reader.TryTake(out ch) || ch != ']' || reader.UnreadCount != 0)
            return false;

        size = new(x, y);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, out Size<T> size)
        => TryParse(text, numberStyle, default, out size);
    
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles numberStyle, IFormatProvider? provider, out Size<T> size)
    {
        size = Empty; // for fast return
        SpanReader<char> reader = new(text);
        if (!reader.TryTake(out char ch) || ch != '[') return false;

        var xText = reader.TakeUntil('×');
        if (!T.TryParse(xText, numberStyle, provider, out var x))
            return false;

        if (!reader.TryTake(out ch) || ch != '×') return false;

        var yText = reader.TakeUntil(']');
        if (!T.TryParse(yText, numberStyle, provider, out var y))
            return false;

        if (!reader.TryTake(out ch) || ch != ']' || reader.UnreadCount != 0)
            return false;

        size = new(x, y);
        return true;
    }

    public static Size<T> Parse([AllowNull, NotNull] string? str, IFormatProvider? provider = default)
        => TryParse(str, provider, out var size) ? size : throw ParseException.CreateFor<T>(str);
    
    public static Size<T> Parse([AllowNull, NotNull] string? str, NumberStyles numberStyles, IFormatProvider? provider = default)
        => TryParse(str, numberStyles, provider, out var size) ? size : throw ParseException.CreateFor<T>(str);

    public static Size<T> Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
        => TryParse(text, provider, out var size) ? size : throw ParseException.CreateFor<T>(text);
    
    public static Size<T> Parse(ReadOnlySpan<char> text, NumberStyles numberStyles, IFormatProvider? provider = default)
        => TryParse(text, numberStyles, provider, out var size) ? size : throw ParseException.CreateFor<T>(text);
#endif


    /// <summary>
    /// Gets the width
    /// </summary>
    public readonly T Width;

    /// <summary>
    /// Gets the height
    /// </summary>
    public readonly T Height;

    public bool IsEmpty
    {
        get
        {
#if NET7_0_OR_GREATER
            return Width == T.Zero && Height == T.Zero;
#else
            return EqualityComparer<T>.Default.Equals(this.Width, default(T)!) 
                && EqualityComparer<T>.Default.Equals(this.Height, default(T)!);
#endif
        }
    }

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
    
    public Size<T> Clone() => new(Width, Height);

    public bool Equals(Size<T> size)
    {
#if NET7_0_OR_GREATER
        return this.Width == size.Width && this.Height == size.Height;
#else
        return EqualityComparer<T>.Default.Equals(this.Width, size.Width) 
            && EqualityComparer<T>.Default.Equals(this.Height, size.Height);
#endif
    }

    public override bool Equals(object? obj)
    {
        return obj is Size<T> size && Equals(size);
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(Width, Height);
    }

    public override string ToString()
    {
        return $"[{Width}×{Height}]";
    }

#if NET6_0_OR_GREATER
    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        charsWritten = 0; // fast return
        SpanWriter<char> writer = stackalloc char[destination.Length];
        
        if (!writer.TryWrite('[')) return false;
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
    
    public string ToString(string? format, IFormatProvider? provider = default)
    {
        var text = new InterpolatedText(3, 2);
        text.Write('[');
        text.Write<T>(Width, format, provider);
        text.Write('×');
        text.Write<T>(Height, format, provider);
        text.Write(']');
        return text.ToStringAndDispose();
    }
}