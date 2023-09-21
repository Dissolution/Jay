using System.Drawing;
using System.Runtime.InteropServices;

namespace Jay.Terminals.Native;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct Rect32 : IEquatable<Rect32>
{
    public static implicit operator Rectangle(Rect32 rect) => new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);

    public static implicit operator Rect32(Rectangle rectangle) => new Rect32(rectangle);

    public static bool operator ==(Rect32 left, Rect32 right) => left.Equals(right);

    public static bool operator !=(Rect32 left, Rect32 right) => !left.Equals(right);


    [FieldOffset(0)]
    public int Left;

    [FieldOffset(4)]
    public int Top;

    [FieldOffset(8)]
    public int Right;

    [FieldOffset(12)]
    public int Bottom;

    public int X
    {
        get => Left;
        set
        {
            Right -= (Left - value);
            Left = value;
        }
    }

    public int Y
    {
        get => Top;
        set
        {
            Bottom -= (Top - value);
            Top = value;
        }
    }

    public int Height
    {
        get => Bottom - Top;
        set => Bottom = value + Top;
    }

    public int Width
    {
        get => Right - Left;
        set => Right = value + Left;
    }

    public Point Location
    {
        get => new Point(Left, Top);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Size Size
    {
        get => new Size(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }
    
    public Rect32(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public Rect32(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
    {
        
    }

    public bool Equals(Rectangle rectangle)
    {
        return rectangle.Left == this.Left &&
               rectangle.Top == this.Top &&
               rectangle.Right == this.Right &&
               rectangle.Bottom == this.Bottom;
    }
    
    public bool Equals(Rect32 rect)
    {
        return rect.Left == Left && rect.Top == Top && rect.Right == Right && rect.Bottom == Bottom;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Rect32 rect) return Equals(rect);
        if (obj is Rectangle rectangle) return Equals(rectangle);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public override string ToString()
    {
        return $"[L:{Left},T:{Top},R:{Right},B:{Bottom}]";
    }
}