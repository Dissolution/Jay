using Jay.Exceptions;
using Jay.Text;

namespace Jay.Dumping.Refactor;

[InterpolatedStringHandler]
public ref struct InterpolatedDumpHandler
{
    private static int GetCapacity(int literalLength, int formattedCount)
        => Math.Max(1024, literalLength + (formattedCount * 16));
    
    private readonly TextBuilder _textBuilder;

    public InterpolatedDumpHandler(int literalLength, int formattedCount)
    {
        _textBuilder = TextBuilder.Borrow(GetCapacity(literalLength, formattedCount));
    }

    public InterpolatedDumpHandler(int literalLength, int formattedCount, TextBuilder textBuilder)
    {
        _textBuilder = textBuilder;
    }
    
    public void AppendLiteral(string? text)
    {
        _textBuilder.Write(text);
    }
    public void AppendFormatted(char ch)
    {
        _textBuilder.Write(ch);
    }
    
    public void AppendFormatted(string? text)
    {
        _textBuilder.Write(text);
    }
    
    public void AppendFormatted(ReadOnlySpan<char> text)
    {
        _textBuilder.Write(text);
    }
    
    public void AppendFormatted(object? value, string? format = null)
    {
        AppendFormatted<object>(value, format);
    }
    
    public void AppendFormatted<T>(T? value, string? format = null)
    {
        Dumping.Dumper.Dump(value, _textBuilder, new DumpOptions{ Format = format});
    }

    public void Dispose()
    {
        _textBuilder.Dispose();
    }

    public string ToStringAndDispose()
    {
        string str = _textBuilder.ToString();
        _textBuilder.Dispose();
        return str;
    }

    public override bool Equals(object? obj) => UnsuitableException.ThrowEquals(typeof(Dumping.Dumper));

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(typeof(Dumping.Dumper));

    public override string ToString() => _textBuilder.ToString();
}