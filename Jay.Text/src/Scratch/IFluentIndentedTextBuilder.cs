namespace Jay.Text.Scratch;

public interface IFluentIndentedTextBuilder<B> : IFluentTextBuilder<B>
    where B : IFluentIndentedTextBuilder<B>
{
    B Indented(char indent, Action<B> buildIndentedText);
    B Indented(string indent, Action<B> buildIndentedText);
    B Indented(scoped ReadOnlySpan<char> indent, Action<B> buildIndentedText);
}