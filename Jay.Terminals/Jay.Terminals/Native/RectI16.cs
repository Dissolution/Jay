using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Validation;

namespace Jay.Terminals.Native;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct RectI16 : 
    IEqualityOperators<RectI16, RectI16, bool>,
    IEquatable<RectI16>
{
    public static bool operator ==(RectI16 a, RectI16 b) => a.Equals(b);
    public static bool operator !=(RectI16 a, RectI16 b) => !a.Equals(b);
        
    public static RectI16 FromLTRB(short left, short top, short right, short bottom)
    {
        Validate.IsPositiveI16(right - left);
        Validate.IsPositiveI16(bottom - top);
        return new RectI16(left, top, right, bottom);
    }
    public static RectI16 FromLTWH(short left, short top, short width, short height)
    {
        Validate.IsPositiveI16(width);
        short right = Validate.AsI16(left + width);
        Validate.IsPositiveI16(height);
        short bottom = Validate.AsI16(top + height);
        return new RectI16(left, top, right, bottom);
    }

    public static RectI16 FromPointSize(PointI16 point, SizeI16 size) 
        => FromLTWH(point.X, point.Y, size.Width, size.Height);

    public static RectI16 FromLTRB32(int left, int top, int right, int bottom)
        => FromLTRB(
            Validate.AsI16(left),
            Validate.AsI16(top),
            Validate.AsI16(right),
            Validate.AsI16(bottom));
    
    
    public static RectI16 FromLTWH32(int left, int top, int width, int height) 
        => FromLTWH(
            Validate.AsI16(left),
            Validate.AsI16(top),
            Validate.AsI16(width),
            Validate.AsI16(height));
    
    public static RectI16 Create(Point point, Size size) 
        => FromPointSize(PointI16.Create(point), SizeI16.Create(size));

    public static RectI16 Create(Rectangle rectangle) 
        => FromLTRB32(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

    [FieldOffset(0)]
    public short Left;
    [FieldOffset(2)] 
    public short Top;

    [FieldOffset(0)]
    public PointI16 UpperLeft;
    
    [FieldOffset(4)] 
    public short Right;
    [FieldOffset(6)] 
    public short Bottom;

    [FieldOffset(4)]
    public PointI16 LowerRight;
    
    public short Width => (short)(Right - Left);
    
    public short Height => (short)(Bottom - Top);
    
    public SizeI16 Size => SizeI16.Create(Width, Height);

    private RectI16(short left, short top, short right, short bottom)
    {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }

    public bool Equals(RectI16 rectI16)
    {
        return rectI16.Left == this.Left &&
            rectI16.Top == this.Top &&
            rectI16.Right == this.Right &&
            rectI16.Bottom == this.Bottom;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is RectI16 rect && Equals(rect);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }
    
    public override string ToString()
    {
        return $"[{Left},{Top}) / ({Right},{Bottom}]";
    }
}