using System.Drawing;

namespace Jay.Terminal.Native;

[Flags]
public enum TerminalColor : byte
{
    Black =       0b00000000, //  0  Black
    DarkBlue =    0b00000001, //  1  Blue
    DarkGreen =   0b00000010, //  2  Green
    DarkCyan =    0b00000011, //  3  Cyan = Green | Blue
    DarkRed =     0b00000100, //  4  Red
    DarkMagenta = 0b00000101, //  5  Magenta = Red | Blue
    DarkYellow =  0b00000110, //  6  Yellow = Red | Green
    Gray =        0b00000111, //  7  Gray = Red | Green | Blue
    DarkGray =    0b00001000, //  8  Intense Black
    Blue =        0b00001001, //  9  Intense Blue
    Green =       0b00001010, // 10  Intense Green
    Cyan =        0b00001011, // 11  Intense Cyan = Intense | Green | Blue
    Red =         0b00001100, // 12  Intense Red
    Magenta =     0b00001101, // 13  Intense Magenta = Intense | Red | Blue
    Yellow =      0b00001110, // 14  Intense Yellow = Intense | Red | Green
    White =       0b00001111, // 15  Intense Gray = Intense | Red | Green | Blue
}

public static class TerminalColors
{
    private static readonly Color[] _terminalColorToColorMap;
    private const double _matchEpsilon = 0.001d;

    static TerminalColors()
    {
        _terminalColorToColorMap = new Color[16]
        {
            /* Black       */ Color.FromArgb(0, 0, 0),
            /* DarkBlue    */ Color.FromArgb(0, 0, 128),
            /* DarkGreen   */ Color.FromArgb(0, 128, 0),
            /* DarkCyan    */ Color.FromArgb(0, 128, 128),
            /* DarkRed     */ Color.FromArgb(128, 0, 0),
            /* DarkMagenta */ Color.FromArgb(128, 0, 128),
            /* DarkYellow  */ Color.FromArgb(128, 128, 0),
            /* Gray        */ Color.FromArgb(192, 192, 192),
            /* DarkGray    */ Color.FromArgb(128, 128, 128),
            /* Blue        */ Color.FromArgb(0, 0, 255),
            /* Green       */ Color.FromArgb(0, 255, 0),
            /* Cyan        */ Color.FromArgb(0, 255, 255),
            /* Red         */ Color.FromArgb(255, 0, 0),
            /* Magenta     */ Color.FromArgb(255, 0, 255),
            /* Yellow      */ Color.FromArgb(255, 255, 0),
            /* White       */ Color.FromArgb(255, 255, 255),
        };
    }

    public static TerminalColor FindNearestTerminalColor(Color color)
    {
        // Simple dist check
        TerminalColor closestColor = default;
        double minDelta = double.MaxValue;
            
        for (int i = 0; i < _terminalColorToColorMap.Length; i++)
        {
            var checkColor = _terminalColorToColorMap[i];
            var diff = Math.Pow(checkColor.R - color.R, 2d) +
                       Math.Pow(checkColor.G - color.G, 2d) +
                       Math.Pow(checkColor.B - color.B, 2d);
            if (diff < minDelta)
            {
                closestColor = (TerminalColor) i;
                minDelta = diff;
            }
        }
        return closestColor;
    }

    public static Color ToColor(this TerminalColor terminalColor)
    {
        return _terminalColorToColorMap[(int) terminalColor];
    }
}