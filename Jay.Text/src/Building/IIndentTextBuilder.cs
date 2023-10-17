namespace Jay.Text.Building;

public interface IIndentTextBuilder<out B> : ITextBuilder<B>
    where B : IIndentTextBuilder<B>
{
    B Indented(char indent, Action<B> buildIndentedText);
    B Indented(string indent, Action<B> buildIndentedText);
    B Indented(scoped ReadOnlySpan<char> indent, Action<B> buildIndentedText);
}