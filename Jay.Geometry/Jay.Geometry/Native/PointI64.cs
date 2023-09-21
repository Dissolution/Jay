using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public readonly struct PointI64 : IGeometry<PointI64>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointI64 first, PointI64 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointI64 first, PointI64 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI64 operator +(PointI64 first, PointI64 second)
        => new((long)(first.X + second.X), (long)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI64 operator -(PointI64 first, PointI64 second)
        => new((long)(first.X - second.X), (long)(first.Y - second.Y));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out PointI64 pointU8)
    {
        if (Point.TryParse<long>(text, provider, out var point))
        {
            pointU8 = new(point.X, point.Y);
            return true;
        }
        pointU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly long X;

    [FieldOffset(8)]
    public readonly long Y;

    public bool IsEmpty => X == default && Y == default;

    public PointI64(long x, long y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointI64 Clone() => this;

    public bool Equals(PointI64 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointI64 point && Equals(point);
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