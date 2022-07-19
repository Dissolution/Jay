// ReSharper disable IdentifierTypo

// https://docs.microsoft.com/en-us/windows/win32/winprog/windows-data-types

global using BOOL = System.Int32;
global using BOOLEAN = System.Byte;
global using BYTE = System.Byte;
global using CCHAR = System.Byte;
global using CHAR = System.Byte;
global using DWORD = System.UInt32;
global using DWORDLONG = System.UInt64;
global using DWORD_PTR = System.UInt64;
global using DWORD32 = System.UInt32;
global using DWORD64 = System.UInt64;
global using FLOAT = System.Single;
#if (_WIN64)
global using HALF_PTR = System.Int32;
#else
global using HALF_PTR = System.Int16;
#endif
//global using HANDLE = *void
global using HRESULT = System.Int32;
global using INT = System.Int32;
#if (_WIN64)
global using INT_PTR = System.Int64;
#else
global using INT_PTR = System.Int32;
#endif
global using INT8 = System.SByte;
global using INT16 = System.Int16;
global using INT32 = System.Int32;
global using INT64 = System.Int64;
global using LANGID = System.Int32;
global using LCID = System.UInt32;
global using LCTYPE = System.UInt32;
global using LGRPID = System.UInt32;
global using LONG = System.Int32;
global using LONGLONG = System.Int64;
#if (_WIN64)
global using LONG_PTR = System.Int64;
#else
global using LONG_PTR = System.Int32;
#endif
global using LONG32 = System.Int32;
global using LONG64 = System.Int64;
global using LPARAM = System.Int32;
global using LPBOOL = System.Int32;
global using LPBYTE = System.Byte;

// TODO!




using System.Drawing;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct COLORREF
{
    public static readonly COLORREF rgbRed   =  0x000000FF;
    public static readonly COLORREF rgbGreen =  0x0000FF00;
    public static readonly COLORREF rgbBlue  =  0x00FF0000;
    public static readonly COLORREF rgbBlack =  0x00000000;
    public static readonly COLORREF rgbWhite =  0x00FFFFFF;
    
    [FieldOffset(0)]
    public DWORD Value;

    [FieldOffset(0)]
    public BYTE Red;

    [FieldOffset(1)]
    public BYTE Green;

    [FieldOffset(2)]
    public BYTE Blue;

    public COLORREF(DWORD value)
    {
        this = new COLORREF();
        this.Value = value;
    }

    public COLORREF(BYTE red, BYTE green, BYTE blue)
    {
        this = new COLORREF();
        this.Red = red;
        this.Green = green;
        this.Blue = blue;
    }

    public bool Equals(COLORREF colorref) => colorref.Value == Value;

    public override bool Equals(object? obj)
    {
        return obj is COLORREF colorref && colorref.Value == Value;
    }

    public override int GetHashCode()
    {
        return (int) Value;
    }

    public override string ToString()
    {
        return $"0x{Value:8X}";
    }

    public static implicit operator COLORREF(DWORD value) => new COLORREF(value);
    
    public static bool operator ==(COLORREF left, COLORREF right) => left.Value == right.Value;
    public static bool operator !=(COLORREF left, COLORREF right) => left.Value != right.Value;
    
}