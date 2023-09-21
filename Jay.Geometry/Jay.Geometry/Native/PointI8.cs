using System.Runtime.InteropServices;
using Jay.Extensions;
using Jay.Utilities;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 2)]
public readonly struct PointI8 : IGeometry<PointI8>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PointI8 first, PointI8 second)
        => first.X == second.X && first.Y == second.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PointI8 first, PointI8 second)
        => first.X != second.X || first.X != second.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI8 operator +(PointI8 first, PointI8 second)
        => new((sbyte)(first.X + second.X), (sbyte)(first.Y + second.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointI8 operator -(PointI8 first, PointI8 second)
        => new((sbyte)(first.X - second.X), (sbyte)(first.Y - second.Y));

    public static bool TryParse(string? s, IFormatProvider? provider, out PointI8 point)
    { 
        if (PointHelper.TryParse<sbyte>(s, provider, out var pt))
        {
            point = new(pt.X, pt.Y);
            return true;
        }
        point = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out PointI8 point)
    {
        if (PointHelper.TryParse<sbyte>(s, provider, out var pt))
        {
            point = new(pt.X, pt.Y);
            return true;
        }
        point = default;
        return false;
    }

    [FieldOffset(0)]
    public readonly sbyte X;

    [FieldOffset(1)]
    public readonly sbyte Y;

    public bool IsEmpty => X == default && Y == default;

    public PointI8(sbyte x, sbyte y)
    {
        this.X = x;
        this.Y = y;
    }

    public PointI8 Clone() => this;

    public bool Equals(PointI8 point) => this.X == point.X && this.Y == point.Y;

    public override bool Equals(object? obj)
    {
        return obj is PointI8 point && Equals(point);
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(X, Y);
    }
    
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
        => PointHelper.TryFormat(X, Y, destination, out charsWritten, format, provider);
    
    public string ToString(string? format, IFormatProvider? provider = default)
        => PointHelper.ToString(X, Y, format, provider);

    public override string ToString() => PointHelper.ToString(X, Y);
}