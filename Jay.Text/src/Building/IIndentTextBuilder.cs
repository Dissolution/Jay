namespace Jay.Text.Building;

public interface IIndentTextBuilder<out B> : ITextBuilder<B>
    where B : IIndentTextBuilder<B>
{
    B AddIndent(char indent);
    B AddIndent(string indent);
    B AddIndent(scoped ReadOnlySpan<char> indent);

    B RemoveIndent();
    B RemoveIndent(out ReadOnlySpan<char> lastIndent);
    
    B Indented(char indent, Action<B> buildIndentedText);
    B Indented(string indent, Action<B> buildIndentedText);
    B Indented(scoped ReadOnlySpan<char> indent, Action<B> buildIndentedText);
}