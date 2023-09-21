using System.Runtime.InteropServices;

namespace Jay.Geometry.Native;

[StructLayout(LayoutKind.Explicit, Size = 2)]
public readonly struct SizeI8 : IGeometry<SizeI8>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SizeI8 first, SizeI8 second)
        => first.Width == second.Width && first.Height == second.Height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SizeI8 first, SizeI8 second)
        => first.Width != second.Width || first.Width != second.Width;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeI8 operator +(SizeI8 first, SizeI8 second)
        => new((sbyte)(first.Width + second.Width), (sbyte)(first.Height + second.Height));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeI8 operator -(SizeI8 first, SizeI8 second)
        => new((sbyte)(first.Width - second.Width), (sbyte)(first.Height - second.Height));
    
    public static bool TryParse(
        ReadOnlySpan<char> text, 
        IFormatProvider? provider, 
        out SizeI8 sizeU8)
    {
        if (Size.TryParse<sbyte>(text, provider, out var size))
        {
            sizeU8 = new(size.Width, size.Height);
            return true;
        }
        sizeU8 = default;
        return false;
    }
    
    [FieldOffset(0)]
    public readonly sbyte Width;

    [FieldOffset(1)]
    public readonly sbyte Height;

    public bool IsEmpty => Width == default && Height == default;

    public SizeI8(sbyte x, sbyte y)
    {
        this.Width = x;
        this.Height = y;
    }

    public SizeI8 Clone() => this;

    public bool Equals(SizeI8 size) => this.Width == size.Width && this.Height == size.Height;

    public override bool Equals(object? obj)
    {
        return obj is SizeI8 size && Equals(size);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }
    
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
        => Size.TryFormat(Width, Height, default, out charsWritten, format, provider);
    
    public string ToString(string? format, IFormatProvider? provider = default) => Size.ToString(Width, Height, format, provider);

    public override string ToString() => Size.ToString(Width, Height);
}