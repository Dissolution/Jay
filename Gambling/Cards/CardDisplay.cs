namespace Jay.Gambling.Cards
{
    public enum CardDisplay
    {
        Default,
        UnicodeSymbol,
        UnicodeSymbols,
    }

    public sealed record Display(string Default, string UnicodeSymbol, string UnicodeSymbols);
}