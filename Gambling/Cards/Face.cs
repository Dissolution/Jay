using System;

namespace Jay.Gambling.Cards
{
    [Flags]
    public enum Face : byte // 1 bit
    {
        Down = 0b0,
        Up = 0b1,
    }
}