using System.Drawing;
using Jay.Utilities;

namespace Jay.Terminals;

/// <summary>
/// Manages <see cref="TerminalColor"/> to <see cref="Color"/> conversion
/// </summary>
public static class TerminalColorMap
{
    private static readonly Color[] _terminalColorToColorMap;

    static TerminalColorMap()
    {
        _terminalColorToColorMap = new Color[16]
        {
            Color.FromArgb(0x00, 0x00, 0x00), // Black
            Color.FromArgb(0x00, 0x00, 0x80), // DarkBlue
            Color.FromArgb(0x00, 0x80, 0x00), // DarkGreen
            Color.FromArgb(0x00, 0x80, 0x80), // DarkCyan
            Color.FromArgb(0x80, 0x00, 0x00), // DarkRed
            Color.FromArgb(0x80, 0x00, 0x80), // DarkMagenta
            Color.FromArgb(0x80, 0x80, 0x00), // DarkYellow
            Color.FromArgb(0xC0, 0xC0, 0xC0), // Gray
            Color.FromArgb(0x80, 0x80, 0x80), // DarkGray
            Color.FromArgb(0x00, 0x00, 0xFF), // Blue
            Color.FromArgb(0x00, 0xFF, 0x00), // Green
            Color.FromArgb(0x00, 0xFF, 0xFF), // Cyan
            Color.FromArgb(0xFF, 0x00, 0x00), // Red
            Color.FromArgb(0xFF, 0x00, 0xFF), // Magenta
            Color.FromArgb(0xFF, 0xFF, 0x00), // Yellow
            Color.FromArgb(0xFF, 0xFF, 0xFF), // White
        };
    }

    /// <summary>
    /// Gets the closest <see cref="TerminalColor"/> match for the given <paramref name="color"/>
    /// </summary>
    public static TerminalColor ToTerminalColor(this Color color)
    {
        // Simple dist check
        TerminalColor closestColor = default;
        double minDelta = double.MaxValue;

        for (int i = 0; i < _terminalColorToColorMap.Length; i++)
        {
            Color checkColor = _terminalColorToColorMap[i];
            var diff = Math.Pow(checkColor.R - color.R, 2d) +
                Math.Pow(checkColor.G - color.G, 2d) +
                Math.Pow(checkColor.B - color.B, 2d);
            if (diff < minDelta)
            {
                closestColor = (TerminalColor)i;
                minDelta = diff;
            }
        }
        return closestColor;
    }

    /// <summary>
    /// Gets the <see cref="Color"/> representation of this <paramref name="terminalColor"/>
    /// </summary>
    public static Color ToColor(this TerminalColor terminalColor)
    {
        return _terminalColorToColorMap[(int)terminalColor];
    }

    public static Color ToColor(this ConsoleColor color)
    {
        return _terminalColorToColorMap[(int)color];
    }

    public static TerminalColor ToTerminalColor(this ConsoleColor consoleColor)
    {
        return (TerminalColor)consoleColor;
    }
    
    public static ConsoleColor ToConsoleColor(this TerminalColor terminalColor)
    {
        return (ConsoleColor)terminalColor;
    }
}