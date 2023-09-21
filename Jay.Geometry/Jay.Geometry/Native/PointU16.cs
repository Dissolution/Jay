using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly struct PointU16 : IGeometry<PointU16>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointU16 first, PointU16 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointU16 first, PointU16 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU16 operator +(PointU16 first, PointU16 second)
        => new((ushort)(first.X + second.X), (ushort)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU16 operator -(PointU16 first, PointU16 second)
        => new((ushort)(first.X - second.X), (ushort)(first.Y - second.Y));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out PointU16 pointU8)
    {
        if (Point.TryParse<ushort>(text, provider, out var point))
        {
            pointU8 = new(point.X, point.Y);
            return true;
        }
        pointU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly ushort X;

    [FieldOffset(2)]
    public readonly ushort Y;

    public bool IsEmpty => X == default && Y == default;

    public PointU16(ushort x, ushort y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointU16 Clone() => this;

    public bool Equals(PointU16 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointU16 point && Equals(point);
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