using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 2)]
public readonly struct PointU8 : IGeometry<PointU8>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointU8 first, PointU8 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointU8 first, PointU8 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU8 operator +(PointU8 first, PointU8 second)
        => new((byte)(first.X + second.X), (byte)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointU8 operator -(PointU8 first, PointU8 second)
        => new((byte)(first.X - second.X), (byte)(first.Y - second.Y));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out PointU8 pointU8)
    {
        if (Point.TryParse<byte>(text, provider, out var point))
        {
            pointU8 = new(point.X, point.Y);
            return true;
        }
        pointU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly byte X;

    [FieldOffset(1)]
    public readonly byte Y;

    public bool IsEmpty => X == default && Y == default;

    public PointU8(byte x, byte y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointU8 Clone() => this;

    public bool Equals(PointU8 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointU8 point && Equals(point);
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