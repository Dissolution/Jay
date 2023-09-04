// ReSharper disable UnusedParameter.Local

namespace Jay.Reflection.CodeBuilding;

[InterpolatedStringHandler]
public ref struct InterpolatedCode
{
    // public static implicit operator InterpolatedCode(string? text)
    // {
    //     var builder = new InterpolatedCode();
    //     builder.AppendFormatted(text);
    //     return builder;
    // }
    //

    private readonly CodeBuilder _code;

    public InterpolatedCode()
    {
        _code = new();
    }

    public InterpolatedCode(int literalLength, int formattedCount)
    {
        _code = new();
    }

    public InterpolatedCode(int literalLength, int formattedCount, CodeBuilder code)
    {
        _code = code;
    }

    public void AppendLiteral(string literal)
    {
        _code.Code(literal);
    }

    public void AppendFormatted(char ch)
    {
        _code.Code(ch.AsSpan());
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _code.Code(text);
    }

    public void AppendFormatted(params char[]? chars)
    {
        _code.Code(chars);
    }

    public void AppendFormatted(string? str)
    {
        _code.Code(str);
    }

    public void AppendFormatted<T>(T? value)
    {
        _code.Code<T>(value);
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        _code.Format<T>(value, format);
    }

    public void Dispose()
    {
        _code.Dispose();
        this = default;
    }

    public string ToStringAndDispose()
    {
        var str = _code.ToString();
        Dispose();
        return str;
    }

    public override string ToString()
    {
        return _code.ToString();
    }

    public override bool Equals(object? obj) => throw new NotSupportedException();
    public override int GetHashCode() => throw new NotSupportedException();
}