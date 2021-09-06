using System;

namespace Jay.Sandbox.Native
{
    /// <summary>
    /// Represents a <see cref="Coxel"/>'s Foreground or Background Color
    /// </summary>
    /// <see cref="System.ConsoleColor"/>
    [Flags]
    public enum CoxelColor : byte
    {
        Black =         0b_0000_0000,
        DarkBlue =      0b_0000_0001,
        DarkGreen =     0b_0000_0010,
        DarkCyan =      0b_0000_0011,   // = DarkGreen | DarkBlue
        DarkRed =       0b_0000_0100,
        DarkMagenta =   0b_0000_0101,   // = DarkRed | DarkBlue
        DarkYellow =    0b_0000_0110,   // = DarkRed | DarkGreen
        Gray =          0b_0000_0111,   // = DarkRed | DarkGreen | DarkBlue
        DarkGray =      0b_0000_1000,   // = Intense | Black
        Blue =          0b_0000_1001,   // = Intense | DarkBlue
        Green =         0b_0000_1010,   // = Intense | DarkGreen
        Cyan =          0b_0000_1011,   // = Intense | DarkGreen | DarkBlue
        Red =           0b_0000_1100,   // = Intense | DarkRed
        Magenta =       0b_0000_1101,   // = Intense | DarkRed | DarkBlue
        Yellow =        0b_0000_1110,   // = Intense | DarkRed | DarkGreen
        White =         0b_0000_1111,   // = Intense | DarkRed | DarkGreen | DarkBlue
    }
}