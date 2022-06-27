using System.Drawing;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace Jay.Terminal.Native;

/// <summary>
/// Win32/GDI Color Structure
/// </summary>
/// <see cref="https://docs.microsoft.com/en-us/windows/win32/gdi/colorref"/>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal struct COLORREF 
{
    [FieldOffset(0)]
    public byte R;
    [FieldOffset(1)]
    public byte G;
    [FieldOffset(2)]
    public byte B;

    [FieldOffset(0)]
    public uint Value;

    public COLORREF(byte r, byte g, byte b) 
    {
        Value = 0;
        R = r;
        G = g;
        B = b;
    }

    public COLORREF(uint value) 
    {
        R = 0;
        G = 0;
        B = 0;
        Value = value & 0x00FFFFFF;
    }

    public COLORREF(Color color)
    {
        Value = (uint) color.ToArgb();
        R = color.R;
        G = color.G;
        B = color.B;
        // R = G = B = 0;
        // Value = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
    }

    public Color ToColor()
    {
        return Color.FromArgb(R, G, B);
    }

    public bool Equals(COLORREF colorref) => this.Value == colorref.Value;

    public bool Equals(Color color)
    {
        return this.R == color.R &&
               this.G == color.G &&
               this.B == color.B;
    }

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

    /// <inheritdoc />
    public override string ToString()
    {
        // 0x00BBGGRR
        return $"0x{Value:X8}";
    }

    public static implicit operator Color(COLORREF colorref) => colorref.ToColor();
    public static implicit operator COLORREF(Color color) => new COLORREF(color);
    public static bool operator ==(COLORREF x, COLORREF y) => x.Equals(y);
    public static bool operator !=(COLORREF x, COLORREF y) => !(x == y);
}