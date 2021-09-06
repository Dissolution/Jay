using System;

namespace Jay.Gambling.Cards
{
    [Flags]
    public enum Suit : byte // 2 bits
    {
        Spade =   0b00,
        Heart =   0b01,
        Club =    0b10,
        Diamond = 0b11,
    }
}