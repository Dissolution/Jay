namespace Jay.Text.Scratch;

[InterpolatedStringHandler]
public ref struct InterpolatedTextBuffer
{
    private readonly TextBuffer _textBuffer;
    
    internal InterpolatedTextBuffer(int literalLength, int formattedCount, TextBuffer textBuffer)
    {
        _textBuffer = textBuffer;
    }
    
    public void AppendLiteral(string literal)
    {
        _textBuffer.Write(literal.AsSpan());
    }

    public void AppendFormatted(char ch)
    {
        _textBuffer.Write(ch);
    }
    
    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _textBuffer.Write(text);
    }
   
    public void AppendFormatted(string? str)
    {
        _textBuffer.Write(str.AsSpan());
    }
    
    public void AppendFormatted<T>(T? value)
    {
        _textBuffer.Format<T>(value);
    }
    
    public void AppendFormatted<T>(T? value, string? format)
    {
        _textBuffer.Format<T>(value, format);
    }
    
    public void AppendFormatted<T>(T? value, ReadOnlySpan<char> format)
    {
        _textBuffer.Format<T>(value, format);
    }
    
    public override bool Equals(object? obj) => throw new NotSupportedException();
    
    public override int GetHashCode() => throw new NotSupportedException();
    
    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}