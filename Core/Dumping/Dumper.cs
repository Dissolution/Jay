using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Text;

namespace Jay.Dumping;

[InterpolatedStringHandler]
public ref struct DumpStringHandler
{
    private TextBuilder _text;

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
        _text.AppendDump<T>(value, DumpLevel.Self);
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

public abstract class Dumper : IDumper
{
    protected internal static bool DumpNull<T>(TextBuilder text, [NotNullWhen(false)] T? value, DumpLevel level)
    {
        if (value is null)
        {
            if (level.HasFlag<DumpLevel>(DumpLevel.Surroundings))
            {
                text.Append('(')
                    .AppendDump(typeof(T), DumpLevel.Self)
                    .Write(')');
            }
            text.Write("null");
            return true;
        }
        return false;
    }

    public abstract bool CanDump(Type type);

    public abstract void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self);
}

public abstract class Dumper<T> : Dumper, IDumper<T>
{
    public sealed override bool CanDump(Type type) => type.IsAssignableTo(typeof(T));

    public sealed override void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self)
    {
        if (value is T typed)
        {
            Dump(text, typed, level);
        }
        else
        {
            text.AppendDump(value, level);
        }
    }

    public abstract void Dump(TextBuilder text, T? value, DumpLevel level = DumpLevel.Self);
}