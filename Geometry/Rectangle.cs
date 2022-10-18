using System.Numerics;

namespace Jay.Geometry;

public readonly struct Rectangle<T> : IEquatable<Rectangle<T>>
    where T : INumber<T>, IMinMaxValue<T>
{
    public static bool operator ==(Rectangle<T> first, Rectangle<T> second) => first.Equals(second);
    public static bool operator !=(Rectangle<T> first, Rectangle<T> second) => !first.Equals(second);

    public static Rectangle<T> Empty { get; } = new Rectangle<T>();
        
    public static Rectangle<T> FromLTRB(T left, T top, T right, T bottom)
    {
        return new Rectangle<T>(left, top, left + right, top + bottom);
    }

    public static Rectangle<T> FromLTWH(T left, T top, T width, T height)
    {
        return new Rectangle<T>(left, top, width, height);
    }

    public static Rectangle<T> FromPointSize(Point<T> location, Size<T> size)
    {
        return new Rectangle<T>(location.X, location.Y, size.Width, size.Height);
    }

    public static Rectangle<T> FromPoints(params Point<T>[] points) => FromPoints((IEnumerable<Point<T>>) points);
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
    public static Rectangle<T> FromRectangles(params Rectangle<T>[] rectangles) => FromRectangles((IEnumerable<Rectangle<T>>) rectangles);
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

    public Point<T> Position => new Point<T>(Left, Top);
    public Size<T> Size => new Size<T>(Width, Height);
        
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

    public bool Contains(Point<T> point)
    {
        (T x, T y) = point;
        return x.CompareTo(Left) >= 0 &&
               x.CompareTo(Right) <= 0 &&
               y.CompareTo(Top) >= 0 &&
               y.CompareTo(Bottom) <= 0;
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

    public override string ToString()
    {
        return $"[{Left},{Top}-{Width}x{Height}]";
    }
}