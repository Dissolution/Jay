using System;

namespace Jay.UI.Terminal
{
    [Flags]
    public enum TerminalColor : byte
    {
        Black =       0b00000000,  // Black
        DarkBlue =    0b00000001,  // Blue
        DarkGreen =   0b00000010,  // Green
        DarkCyan =    0b00000011,  // Cyan = Green | Blue
        DarkRed =     0b00000100,  // Red
        DarkMagenta = 0b00000101,  // Magenta = Red | Blue
        DarkYellow =  0b00000110,  // Yellow = Red | Green
        Gray =        0b00000111,  // Gray = Red | Green | Blue
        DarkGray =    0b00001000,  // Intense Black
        Blue =        0b00001001,  // Intense Blue
        Green =       0b00001010,  // Intense Green
        Cyan =        0b00001011,  // Intense Cyan = Intense | Green | Blue
        Red =         0b00001100,  // Intense Red
        Magenta =     0b00001101,  // Intense Magenta = Intense | Red | Blue
        Yellow =      0b00001110,  // Intense Yellow = Intense | Red | Green
        White =       0b00001111,  // Intense Gray = Intense | Red | Green | Blue
    }
}