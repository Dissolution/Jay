using Jay.Hashing;
using Jay.Reflection;
using System;
using System.Collections.Generic;

namespace Jay.Geometry;

public readonly struct Rectangle : IEquatable<Rectangle>
{
    public static readonly Rectangle Empty = default;
        
    public static Rectangle FromLTRB(int left, int top, int right, int bottom)
    {
        return new Rectangle(left, top, Unmanaged.Add(left, right), Unmanaged.Add(top, bottom));
    }

    public static Rectangle FromLTWH(int left, int top, int width, int height)
    {
        return new Rectangle(left, top, width, height);
    }

    public static Rectangle FromPointSize(Point location, Size size)
    {
        return new Rectangle(location.X, location.Y, size.Width, size.Height);
    }

    public static Rectangle FromPoints(params Point[] points) => FromPoints((IEnumerable<Point>) points);
    public static Rectangle FromPoints(IEnumerable<Point> points)
    {
        int left = int.MaxValue;
        int top = int.MaxValue;
        int right = int.MinValue;
        int bottom = int.MinValue;
        foreach ((int x, int y) in points)
        {
            if (x.CompareTo(left) < 0)
                left = x;
            if (x.CompareTo(right) > 0)
                right = x;
            if (y.CompareTo(top) < 0)
                top = y;
            if (y.CompareTo(bottom) > 0)
                bottom = y;
        }
        return FromLTRB(left, top, right, bottom);
    }

    public readonly int X;
    public readonly int Y;
    public readonly int Width;
    public readonly int Height;

    public int Left => X;
    public int Top => Y;
    public int Right => Left + Width;
    public int Bottom => Top + Height;

    public Point Position => new Point(Left, Top);
    public Size Size => new Size(Width, Height);
        
    private Rectangle(int left, int top, int width, int height)
    {
        this.X = left;
        this.Y = top;
        this.Width = width;
        this.Height = height;
    }

    public void Deconstruct(out int x, out int y, out int width, out int height)
    {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    public bool Contains(Point point)
    {
        (int x, int y) = point;
        return x.CompareTo(Left) >= 0 &&
               x.CompareTo(Right) <= 0 &&
               y.CompareTo(Top) >= 0 &&
               y.CompareTo(Bottom) <= 0;
    }
        
    public bool Equals(Rectangle rectangle)
    {
        return rectangle.Left == Left &&
               rectangle.Top == Top &&
               rectangle.Right == Right &&
               rectangle.Bottom == Bottom;
    }

    public override bool Equals(object? obj)
    {
        return obj is Rectangle rectangle && Equals(rectangle);
    }

    public override int GetHashCode()
    {
        return Hasher.Create(Left, Top, Right, Bottom);
    }

    public override string ToString()
    {
        return $"[{Left},{Top}-{Width}x{Height}]";
    }
}