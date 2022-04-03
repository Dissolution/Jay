namespace Lyph;

[Flags]
public enum Color : byte
{
    None =      0b00000000,
    Blue =      0b00001001,
    Green =     0b00001010,
    Cyan =      0b00001011,
    Red =       0b00001100,
    Magenta =   0b00001101,
    Yellow =    0b00001110,
    White =     0b00001111,
}