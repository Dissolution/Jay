namespace Jay.Text.Building;

[InterpolatedStringHandler]
public ref struct InterpolatedFluentTextBuilder<B>
    where B : FluentTextBuilder<B>
{
    private readonly B _textBuilder;
    
    public InterpolatedFluentTextBuilder(int literalLength, int formattedCount, B textBuilder)
    {
        _textBuilder = textBuilder;
    }
    
    public void AppendLiteral(string literal)
    {
        _textBuilder.Append(literal);
    }

    public void AppendFormatted(char ch)
    {
        _textBuilder.Append(ch);
    }
    
    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _textBuilder.Append(text);
    }
   
    public void AppendFormatted(string? str)
    {
        _textBuilder.Append(str.AsSpan());
    }
    
    public void AppendFormatted<T>(T? value)
    {
        _textBuilder.Append<T>(value);
    }
    
    public void AppendFormatted<T>(T? value, string? format)
    {
        _textBuilder.Append<T>(value, format);
    }
    
    public void AppendFormatted<T>(T? value, ReadOnlySpan<char> format)
    {
        _textBuilder.Append<T>(value, format);
    }
    
    public override bool Equals(object? obj) => throw new NotSupportedException();
    
    public override int GetHashCode() => throw new NotSupportedException();
    
    public override string ToString()
    {
        return _textBuilder.ToString();
    }
}