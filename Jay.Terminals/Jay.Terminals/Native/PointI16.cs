using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Validation;

namespace Jay.Terminals.Native;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct PointI16 : 
    IEqualityOperators<PointI16, PointI16, bool>,
    IEquatable<PointI16>
{
    public static bool operator ==(PointI16 a, PointI16 b) => a.Equals(b);
    public static bool operator !=(PointI16 a, PointI16 b) => !a.Equals(b);

    public static readonly PointI16 Empty = default;

    public static PointI16 Create(Point point) 
        => new(Validate.AsI16(point.X), Validate.AsI16(point.Y));
    
    public static PointI16 Create(short x, short y) => new(x, y);

    public static PointI16 Create(int x, int y) 
        => new(Validate.AsI16(x), Validate.AsI16(y));

    [FieldOffset(0)]
    private int _value;
    
    [FieldOffset(0)] 
    public short X;
    
    [FieldOffset(2)] 
    public short Y;

    public bool IsEmpty => X == 0 && Y == 0;
    
    public PointI16(short x, short y)
    {
        this.X = x;
        this.Y = y;
    }

    public void Deconstruct(out short x, out short y)
    {
        x = this.X;
        y = this.Y;
    }
    
    public bool Equals(PointI16 pointI16) => X == pointI16.X && Y == pointI16.Y;

    public override bool Equals(object? obj) => obj is PointI16 point && Equals(point);

    public override int GetHashCode() => _value;

    public override string ToString() => $"({X},{Y})";
}