namespace Jay.Gambling.Cards
{
    public enum Rank : byte // 4 bits
    {
        // 0
        Ace =    0b0001,
        Two =    0b0010,
        Three =  0b0011,
        Four =   0b0100,
        Five =   0b0101,
        Six =    0b0110,
        Seven =  0b0111,
        Eight =  0b1000,
        Nine =   0b1001,
        Ten =    0b1010,
        Jack =   0b1011,
        Queen =  0b1100,
        King =   0b1101,
        Knight = 0b1110,
        Joker =  0b1111,
    }
}