// ReSharper disable UnusedParameter.Local

using TextWriter = Jay.Text.Building.TextWriter;

namespace Jay.Reflection.CodeBuilding;

[InterpolatedStringHandler]
public ref struct InterpolatedCodeBuilder
{
    public static implicit operator InterpolatedCodeBuilder(string? text)
    {
        var builder = new InterpolatedCodeBuilder();
        builder.AppendFormatted(text);
        return builder;
    }
    
    
    private readonly CodeBuilder _codeBuilder;

    public InterpolatedCodeBuilder()
    {
        _codeBuilder = new();
    }
    
    public InterpolatedCodeBuilder(int literalLength, int formattedCount)
    {
        _codeBuilder = new();
    }
    
    public InterpolatedCodeBuilder(int literalLength, int formattedCount, CodeBuilder codeBuilder)
    {
        _codeBuilder = codeBuilder;
    }
    
    public void AppendLiteral(string literal)
    {
        _codeBuilder.Write(literal);
    }

    public void AppendFormatted(char ch)
    {
        _codeBuilder.Write(ch);
    }
    
    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _codeBuilder.Write(text);
    }
    
    public void AppendFormatted(string? str)
    {
        _codeBuilder.Write(str);
    }
    
    public void AppendFormatted<T>(T? value)
    {
        _codeBuilder.Format<T>(value);
    }
    
    public void AppendFormatted<T>(T? value, string? format)
    {
        _codeBuilder.Format<T>(value, format);
    }

    public void Dispose()
    {
        TextWriter? toReturn = _codeBuilder;
        this = default;
        toReturn?.Dispose();
    }

    public string ToStringAndDispose()
    {
        var str = _codeBuilder.ToString();
        this.Dispose();
        return str;
    }
    
    public override string ToString()
    {
        return _codeBuilder.ToString();
    }

    public override bool Equals(object? obj) => throw new NotSupportedException();
    public override int GetHashCode() => throw new NotSupportedException();
}