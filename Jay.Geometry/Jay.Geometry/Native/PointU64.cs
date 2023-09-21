using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public readonly struct PointU64 : IGeometry<PointU64>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointU64 first, PointU64 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointU64 first, PointU64 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU64 operator +(PointU64 first, PointU64 second)
        => new((ulong)(first.X + second.X), (ulong)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU64 operator -(PointU64 first, PointU64 second)
        => new((ulong)(first.X - second.X), (ulong)(first.Y - second.Y));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out PointU64 pointU8)
    {
        if (Point.TryParse<ulong>(text, provider, out var point))
        {
            pointU8 = new(point.X, point.Y);
            return true;
        }
        pointU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly ulong X;

    [FieldOffset(8)]
    public readonly ulong Y;

    public bool IsEmpty => X == default && Y == default;

    public PointU64(ulong x, ulong y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointU64 Clone() => this;

    public bool Equals(PointU64 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointU64 point && Equals(point);
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