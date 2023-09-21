using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly struct PointI32 : IGeometry<PointI32>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointI32 first, PointI32 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointI32 first, PointI32 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI32 operator +(PointI32 first, PointI32 second)
        => new((int)(first.X + second.X), (int)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI32 operator -(PointI32 first, PointI32 second)
        => new((int)(first.X - second.X), (int)(first.Y - second.Y));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out PointI32 pointU8)
    {
        if (Point.TryParse<int>(text, provider, out var point))
        {
            pointU8 = new(point.X, point.Y);
            return true;
        }
        pointU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly int X;

    [FieldOffset(4)]
    public readonly int Y;

    public bool IsEmpty => X == default && Y == default;

    public PointI32(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointI32 Clone() => this;

    public bool Equals(PointI32 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointI32 point && Equals(point);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
        => Point.TryFormat(X, Y, default, out charsWritten, format, provider);
    
    public string ToString(string? format, IFormatProvider? provider = default) => Point.ToString(X, Y, format, provider);

    public override string ToString() => Point.ToString(X, Y);
}