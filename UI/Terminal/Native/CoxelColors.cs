using System;

namespace Jay.UI.Terminal.Native
{
    [Flags]
    public enum CoxelColors : ushort
    {
        Black =            0b00000000, // Black
        
        Fore_Black =       0b00000000, // Black
        Fore_DarkBlue =    0b00000001, // Blue
        Fore_DarkGreen =   0b00000010, // Green
        Fore_DarkCyan =    0b00000011, // Cyan = Green | Blue
        Fore_DarkRed =     0b00000100, // Red
        Fore_DarkMagenta = 0b00000101, // Magenta = Red | Blue
        Fore_DarkYellow =  0b00000110, // Yellow = Red | Green
        Fore_Gray =        0b00000111, // Gray = Red | Green | Blue
        Fore_DarkGray =    0b00001000, // Intense Black
        Fore_Blue =        0b00001001, // Intense Blue
        Fore_Green =       0b00001010, // Intense Green
        Fore_Cyan =        0b00001011, // Intense Cyan = Intense | Green | Blue
        Fore_Red =         0b00001100, // Intense Red
        Fore_Magenta =     0b00001101, // Intense Magenta = Intense | Red | Blue
        Fore_Yellow =      0b00001110, // Intense Yellow = Intense | Red | Green
        Fore_White =       0b00001111, // Intense Gray = Intense | Red | Green | Blue
        
        Back_Black =       0b00000000, // Black
        Back_DarkBlue =    0b00010000, // Blue
        Back_DarkGreen =   0b00100000, // Green
        Back_DarkCyan =    0b00110000, // Cyan = Green | Blue
        Back_DarkRed =     0b01000000, // Red
        Back_DarkMagenta = 0b01010000, // Magenta = Red | Blue
        Back_DarkYellow =  0b01100000, // Yellow = Red | Green
        Back_Gray =        0b01110000, // Gray = Red | Green | Blue
        Back_DarkGray =    0b10000000, // Intense Black
        Back_Blue =        0b10010000, // Intense Blue
        Back_Green =       0b10100000, // Intense Green
        Back_Cyan =        0b10110000, // Intense Cyan = Intense | Green | Blue
        Back_Red =         0b11000000, // Intense Red
        Back_Magenta =     0b11010000, // Intense Magenta = Intense | Red | Blue
        Back_Yellow =      0b11100000, // Intense Yellow = Intense | Red | Green
        Back_White =       0b11110000, // Intense Gray = Intense | Red | Green | Blue
        
        // Common_LVB_Leading_Byte = 0b100000000,
        // Common_LVB_Trailing_Byte = 0b1000000000,
        // Common_LVB_Grid_Horizontal = 0b10000000000,
        // Common_LVB_Grid_LVertical = 0b100000000000,
        // Common_LVB_Grid_RVertical = 0b1000000000000,
        // Common_LVB_Reverse_Video = 0b100000000000000,
        // Common_LVB_Underscore = 0b1000000000000000,
    }
}
