using System.Runtime.InteropServices;

using WORD = System.UInt16;  // Size = 2
using ULONG = System.UInt32; // Size = 4
using BOOL = System.Boolean; // Size = 4

namespace Jay.Terminals.Native;

[StructLayout(LayoutKind.Sequential, Size = 64)]
internal struct ColorTable
{
    public COLORREF Black;
    public COLORREF DarkBlue;
    public COLORREF DarkGreen;
    public COLORREF DarkCyan;
    public COLORREF DarkRed;
    public COLORREF DarkMagenta;
    public COLORREF DarkYellow;
    public COLORREF Gray;
    public COLORREF DarkGray;
    public COLORREF Blue;
    public COLORREF Green;
    public COLORREF Cyan;
    public COLORREF Red;
    public COLORREF Magenta;
    public COLORREF Yellow;
    public COLORREF White;

    public COLORREF GetColorRef(TerminalColor terminalColor)
    {
        return terminalColor switch
        {
            TerminalColor.Black => Black,
            TerminalColor.DarkBlue => DarkBlue,
            TerminalColor.DarkGreen => DarkGreen,
            TerminalColor.DarkCyan => DarkCyan,
            TerminalColor.DarkRed => DarkRed,
            TerminalColor.DarkMagenta => DarkMagenta,
            TerminalColor.DarkYellow => DarkYellow,
            TerminalColor.Gray => Gray,
            TerminalColor.DarkGray => DarkGray,
            TerminalColor.Blue => Blue,
            TerminalColor.Green => Green,
            TerminalColor.Cyan => Cyan,
            TerminalColor.Red => Red,
            TerminalColor.Magenta => Magenta,
            TerminalColor.Yellow => Yellow,
            TerminalColor.White => White,
            _ => throw new ArgumentOutOfRangeException(nameof(terminalColor), terminalColor, null)
        };
    }
}

/// <summary>
/// Contains extended information about a console screen buffer.
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/windows/console/console-screen-buffer-infoex"/>
[StructLayout(LayoutKind.Explicit, Size = 96)]
internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
{
    /// <summary>
    /// The size of this structure, in bytes.
    /// </summary>
    [FieldOffset(0)]
    public ULONG cbSize = 96;
    /// <summary>
    /// A COORD structure that contains the size of the console screen buffer, in character columns and rows.
    /// </summary>
    [FieldOffset(4)]
    public PointI16 dwSize;
    /// <summary>
    /// A COORD structure that contains the column and row coordinates of the cursor in the console screen buffer.
    /// </summary>
    [FieldOffset(8)]
    public PointI16 dwCursorPosition;
    /// <summary>
    /// The attributes of the characters written to a screen buffer by the WriteFile and WriteConsole functions, or echoed to a screen buffer by the ReadFile and ReadConsole functions.
    /// </summary>
    [FieldOffset(12)]
    public CharacterAttributes wAttributes;
    /// <summary>
    /// A SMALL_RECT structure that contains the console screen buffer coordinates of the upper-left and lower-right corners of the display window.
    /// </summary>
    [FieldOffset(14)]
    public RectI16 srWindow;
    /// <summary>
    /// A COORD structure that contains the maximum size of the console window, in character columns and rows, given the current screen buffer size and font and the screen size.
    /// </summary>
    [FieldOffset(22)]
    public PointI16 dwMaximumWindowSize;
    /// <summary>
    /// The fill attribute for console pop-ups.
    /// </summary>
    [FieldOffset(26)]
    public WORD wPopupAttributes;
    /// <summary>
    /// If this member is TRUE, full-screen mode is supported; otherwise, it is not. This will always be FALSE for systems after Windows Vista with the WDDM driver model as true direct VGA access to the monitor is no longer available.
    /// </summary>
    [FieldOffset(28)]
    public BOOL bFullscreenSupported;
    /// <summary>
    /// An array of COLORREF values that describe the console's color settings.
    /// </summary>
    [FieldOffset(32)]
    public ColorTable ColorTable;
    //[FieldOffset(96)]

    public CONSOLE_SCREEN_BUFFER_INFO_EX()
    {
        
    }
}