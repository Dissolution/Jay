
namespace Jay.Text.Scratch;

[InterpolatedStringHandler]
public ref struct InterpolatedTextWriter
{
    public static implicit operator InterpolatedTextWriter(string? text)
    {
        var builder = new InterpolatedTextWriter();
        builder.AppendFormatted(text);
        return builder;
    }
    
    private readonly ITextWriter _textWriter;

    public InterpolatedTextWriter()
    {
        _textWriter = new();
    }
    
    public InterpolatedTextWriter(int literalLength, int formattedCount)
    {
        _textWriter = new(Jay.Text.Building.BuilderHelper.GetInterpolatedStartCapacity(literalLength, formattedCount));
    }
    
    public InterpolatedTextWriter(int literalLength, int formattedCount, ITextWriter textWriter)
    {
        _textWriter = textWriter;
    }
    
    public void AppendLiteral(string literal)
    {
        _textWriter.Write(literal);
    }

    public void AppendFormatted(char ch)
    {
        _textWriter.Write(ch);
    }
    
    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _textWriter.Write(text);
    }
    
    public void AppendFormatted(string? str)
    {
        _textWriter.Write(str);
    }
    
    public void AppendFormatted<T>(T? value)
    {
        _textWriter.Format<T>(value);
    }
    
    public void AppendFormatted<T>(T? value, string? format)
    {
        _textWriter.Format<T>(value, format);
    }

    public void Dispose()
    {
        var toReturn = _textWriter;
        this = default;
        toReturn?.Dispose();
    }

    public string ToStringAndDispose()
    {
        var str = _textWriter?.ToString();
        this.Dispose();
        return str!;
    }
    
    public override string ToString()
    {
        return _textWriter?.ToString()!;
    }

    public override bool Equals(object? obj) => throw new NotSupportedException();
    public override int GetHashCode() => throw new NotSupportedException();
}