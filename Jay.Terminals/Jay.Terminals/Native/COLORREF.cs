using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace Jay.Terminals.Native;

/// <summary>
/// Win32/GDI Color Structure
/// </summary>
/// <see cref="https://docs.microsoft.com/en-us/windows/win32/gdi/colorref"/>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal struct COLORREF
{
    public static implicit operator Color(COLORREF colorref) => colorref.ToColor();
    public static implicit operator COLORREF(Color color) => new COLORREF(color);

    public static bool operator ==(COLORREF x, COLORREF y) => x.Value == y.Value;
    public static bool operator !=(COLORREF x, COLORREF y) => x.Value != y.Value;
    
    [FieldOffset(0)]
    public byte Red;
    [FieldOffset(1)]
    public byte Green;
    [FieldOffset(2)]
    public byte Blue;

    [FieldOffset(3)] 
    internal byte Alpha;

    [FieldOffset(0)]
    public uint Value;

    [SkipLocalsInit]
    public COLORREF(byte red, byte green, byte blue) 
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = 0;
    }

    [SkipLocalsInit]
    public COLORREF(uint value)
    {
        Value = value; // & 0x00FFFFFF;  // if we want to ignore alpha
    }

    [SkipLocalsInit]
    public COLORREF(Color color)
    {
        Value = (uint)color.ToArgb();
    }

    public Color ToColor()
    {
        return Color.FromArgb(Alpha, Red, Green, Blue);
    }

    public bool Equals(COLORREF colorref) => this.Value == colorref.Value;

    public bool Equals(Color color) => this.Value == (uint)color.ToArgb();

    public override bool Equals(object? obj)
    {
        if (obj is COLORREF colorref) return Equals(colorref);
        if (obj is Color color) return Equals(color);
        return false;
    }

    public override int GetHashCode()
    {
        return (int)Value;
    }

    public override string ToString()
    {
        // 0xAABBGGRR
        return $"0x{Value:X8}";
    }

  
}