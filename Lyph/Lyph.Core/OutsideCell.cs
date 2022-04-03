namespace Lyph;

public sealed class OutsideCell : Cell
{
    public static OutsideCell Instance { get; } = new OutsideCell();

    private OutsideCell()
    {
        Glyph = Glyph.FromSymbol(' ');
        Color = Color.None;
        Bleed = 0.0d;
        Drain = 0.0d;
    }
}