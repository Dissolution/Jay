using System.Runtime.CompilerServices;

namespace Jay.Geometry;

public readonly struct Point<T> : IEquatable<Point<T>>
    where T : INumber<T>, IMinMaxValue<T>
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

    public static Point<T> Empty { get; } = new Point<T>();
    
    
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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X},{Y})";
    }
}

