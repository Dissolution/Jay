using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Validation;

namespace Jay.Terminals.Native;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct SizeI16 : 
    IEqualityOperators<SizeI16, SizeI16, bool>,
    IEquatable<SizeI16>
{
    public static bool operator ==(SizeI16 a, SizeI16 b) => a.Equals(b);
    public static bool operator !=(SizeI16 a, SizeI16 b) => !a.Equals(b);

    public static readonly SizeI16 Empty = default;

    public static SizeI16 Create(Size size) => new(Validate.AsPositiveI16(size.Width), Validate.AsPositiveI16(size.Height));
    public static SizeI16 Create(short width, short height) => new(Validate.AsPositiveI16(width), Validate.AsPositiveI16(height));
    public static SizeI16 Create(int width, int height) => new(Validate.AsPositiveI16(width), Validate.AsPositiveI16(height));

    [FieldOffset(0)]
    private int _value;
    
    [FieldOffset(0)] 
    public short Width;
    
    [FieldOffset(2)] 
    public short Height;
    
    public bool IsEmpty => Width == 0 && Height == 0;
    
    private SizeI16(short width, short height)
    {
        Debug.Assert(width >= 0);
        Debug.Assert(height >= 0);
        this.Width = width;
        this.Height = height;
    }

    public void Deconstruct(out short width, out short height)
    {
        width = this.Width;
        height = this.Height;
    }

    public bool Equals(SizeI16 size) => this.Width == size.Width && this.Height == size.Height;

    public override bool Equals(object? obj) => obj is SizeI16 size && Equals(size);

    public override int GetHashCode() => _value;
    
    public override string ToString() => $"[{Width}×{Height}]";
}