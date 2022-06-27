using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Jay.Geometry;

public readonly struct Point : IEquatable<Point>
{
    public static implicit operator PointF(Point p) => new PointF((float)p.X, (float)p.Y);
    public static explicit operator Size(Point p) => new Size(p.X, p.Y);

    public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;
    public static bool operator !=(Point left, Point right) => !(left == right);
    public static Point operator +(Point left, Point right) => Point.Shifted(left, right);
    public static Point operator -(Point left, Point right) => Point.Shifted(left, -right);
    public static Point operator +(Point left, Size right) => Point.Shifted(pt, right);
    public static Point operator -(Point left, Size right) => Point.Shifted(pt, -right);

    public static Point operator -(Point point) => new Point(-point.X, -point.Y);
    

    public static readonly Point Empty = new();
    
    
    public static Point Shifted(Point pt, Point sz) => new Point(pt.X + sz.Width, pt.Y + sz.Height);
    public static Point Shifted(Point pt, Size sz) => new Point(pt.X + sz.Width, pt.Y + sz.Height);

    public static Point Subtract(Point pt, Size sz) => new Point(pt.X - sz.Width, pt.Y - sz.Height);

    public static Point Ceiling(PointF value) => new Point((int)System.Math.Ceiling((double)value.X), (int)System.Math.Ceiling((double)value.Y));

    public static Point Truncate(PointF value) => new Point((int)value.X, (int)value.Y);

    public static Point Round(PointF value) => new Point((int)System.Math.Round((double)value.X), (int)System.Math.Round((double)value.Y));

    
    
    

    public readonly int X;
    public readonly int Y;

    [Browsable(false)]
    public bool IsEmpty => this.X == 0 && this.Y == 0;


    
    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public Point(Size sz)
    {
        this.X = sz.Width;
        this.Y = sz.Height;
    }

    public Point(int dw)
    {
        this.X = (int)Point.LowInt16(dw);
        this.Y = (int)Point.HighInt16(dw);
    }

    public void Deconstruct(out int x, out int y)
    {
        x = this.X;
        y = this.Y;
    }

   
    public Point Shifted(int dx, int dy)
    {
        return new Point(X + dx, Y + dy);
    }

    public Point Shifted(Point p) => this.Shifted(p.X, p.Y);

    
    public bool Equals(Point point)
    {
        return point.X == this.X && point.Y == this.Y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point point) return Equals(point);
        if (obj is Size size) return Equals(size);
        return false;
    }

    public override int GetHashCode() => HashCode.Combine<int, int>(this.X, this.Y);

    
    public override string ToString() => "{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + "}";

    private static short HighInt16(int n) => (short)(n >> 16 & (int)ushort.MaxValue);

    private static short LowInt16(int n) => (short)(n & (int)ushort.MaxValue);
}