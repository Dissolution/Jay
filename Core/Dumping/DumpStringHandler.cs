using System.Runtime.CompilerServices;
using Jay.Exceptions;
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

    public override bool Equals(object? obj)
    {
        return UnsuitableException.ThrowEquals(typeof(DumpStringHandler));
    }

    public override int GetHashCode()
    {
        return UnsuitableException.ThrowGetHashCode(typeof(DumpStringHandler));
    }

    public override string ToString()
    {
        throw new InvalidOperationException($"You MUST call {nameof(ToStringAndClear)}() in order to get the string output of resolving the {nameof(DumpStringHandler)}");
    }
}