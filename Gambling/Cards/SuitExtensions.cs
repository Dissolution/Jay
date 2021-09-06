namespace Jay.Gambling.Cards
{
    public static class SuitExtensions
    {
        public static bool IsRed(this Suit suit) => ((int)suit & 0b01) != 0;
        public static bool IsBlack(this Suit suit) => ((int)suit & 0b01) == 0;
    }
}