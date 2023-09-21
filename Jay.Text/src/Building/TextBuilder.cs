namespace Jay.Text.Building;

public sealed class TextBuilder : 
    FluentTextBuilder<TextBuilder>
{
    public static TextBuilder New => new();
    
    public TextBuilder()
    {
    }
}