using System;
using System.Runtime.CompilerServices;

namespace Jay.CLI.Native
{
    public static class CharInfoAttributesExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerminalColor GetForeColor(this CharInfoAttributes attributes)
        {
            // Get last four bits
            return (TerminalColor)((int)attributes & 0b00001111);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetForeColor(this ref CharInfoAttributes attributes,
                                        TerminalColor color)
        {
            // Mask to clear the last four bits
            const int clearMask = 0b11110000;
            // Clear them
            int cleared = (int) attributes & clearMask;
            CharInfoAttributes clearedAttr = (CharInfoAttributes) cleared;
            // Set last four bits
            int set = (int) color | cleared;
            CharInfoAttributes setAttr = (CharInfoAttributes) set;
            attributes = setAttr;
            
            
            
            //attributes = (CharInfoAttributes)(((int) attributes & 0b11111100) | (int) color);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerminalColor GetBackColor(this CharInfoAttributes attributes)
        {
            // Get last four bits
            return (TerminalColor)(((int)attributes & 0b00001111) >> 4);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBackColor(this ref CharInfoAttributes attributes,
                                        TerminalColor color)
        {
            // Set last four bits
            //attributes = (CharInfoAttributes)(((int) attributes & 0b11110011) | ((int) color << 2));
            
            // Mask to clear the first four bits
            const int clearMask = 0b00001111;
            // Clear them
            int cleared = (int) attributes & clearMask;
            CharInfoAttributes clearedAttr = (CharInfoAttributes) cleared;
            // Set first four bits
            int set = ((int) color<<4) | cleared;
            CharInfoAttributes setAttr = (CharInfoAttributes) set;
            attributes = setAttr;
        }
    }
    
    [Flags]
    public enum CharInfoAttributes : ushort
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
        
        // FOREGROUND_BLUE = 0b00000001,            //	Text color contains blue.
        // FOREGROUND_GREEN = 0b00000010,           //	Text color contains green.
        // FOREGROUND_CYAN = FOREGROUND_GREEN | FOREGROUND_BLUE,
        // FOREGROUND_RED = 0x0004,             //	Text color contains red.
        // FOREGROUND_INTENSITY = 0x0008,       //	Text color is intensified.
        //
        // BACKGROUND_BLUE = 0x0010,            //	Background color contains blue.
        // BACKGROUND_GREEN = 0x0020,           //	Background color contains green.
        // BACKGROUND_RED = 0x0040,             //	Background color contains red.
        // BACKGROUND_INTENSITY = 0x0080,       //	Background color is intensified.
        //
        // // All below only supported in CJK languages
        // COMMON_LVB_LEADING_BYTE = 0x0100,    //	Leading byte.
        // COMMON_LVB_TRAILING_BYTE = 0x0200,   //	Trailing byte.
        //
        // COMMON_LVB_GRID_HORIZONTAL = 0x0400, //	Top horizontal.
        // COMMON_LVB_GRID_LVERTICAL = 0x0800,  //	Left vertical.
        // COMMON_LVB_GRID_RVERTICAL = 0x1000,  //	Right vertical.
        //
        // COMMON_LVB_REVERSE_VIDEO = 0x4000,   //	Reverse foreground and background attribute.
        //
        // COMMON_LVB_UNDERSCORE = 0x8000,      //	Underscore.
    }
}