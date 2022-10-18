using System.Numerics;
using System.Runtime.CompilerServices;

namespace Jay.Geometry;

public readonly struct Size<T> : IEquatable<Size<T>>
    where T : INumber<T>, IMinMaxValue<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Size<T> first, Size<T> second) => first.Width == second.Width && first.Height == second.Height;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Size<T> first, Size<T> second) => first.Width != second.Width || first.Height != second.Height;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator +(Size<T> first, Size<T> second) => new Size<T>(first.Width + second.Width, first.Height + second.Height);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator -(Size<T> first, Size<T> second) => new Size<T>(first.Width - second.Width, first.Height - second.Height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size<T> operator -(Size<T> point) => new Size<T>(-point.Width, -point.Height);

    public static Size<T> Empty { get; } = new Size<T>();
    
    
    public readonly T Width;
    public readonly T Height;

    public bool HasArea => Width > T.Zero && Height > T.Zero;
    
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

    /// <inheritdoc />
    public bool Equals(Size<T> size)
    {
        return this.Width == size.Width && this.Height == size.Height;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Size<T> size) return Equals(size);
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Width}x{Height}]";
    }
}