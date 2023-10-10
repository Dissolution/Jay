namespace Jay.Text.Scratch;

public static class FluentTextBuilderExtensions
{
    public static B NewLines<B>(this B textBuilder, int count)
        where B : IFluentTextBuilder<B>
    {
        for (var i = 0; i < count; i++)
        {
            textBuilder.NewLine();
        }
        return textBuilder;
    }

    public static B Invoke<B>(this B textBuilder, Action<B> buildText)
        where B : IFluentTextBuilder<B>
    {
        buildText(textBuilder);
        return textBuilder;
    }
}