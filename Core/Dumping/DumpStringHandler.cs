using System.Runtime.CompilerServices;
using Jay.Text;

namespace Jay.Dumping;

[InterpolatedStringHandler]
public ref struct DumpStringHandler
{
    private readonly TextBuilder _text;

    public DumpStringHandler(int literalLength, int formatCount)
    {
        _text = new TextBuilder();
    }

    public void AppendLiteral(string? str)
    {
        _text.Write(str);
    }

    public void AppendFormatted<T>(T? value)
    {
        _text.AppendDump<T>(value);
    }

    public string ToStringAndClear()
    {
        var str = _text.ToString();
        _text.Dispose();
        return str;
    }

    public void Dispose()
    {
        _text.Dispose();
    }

    public override string ToString()
    {
        return _text.ToString();
    }
}