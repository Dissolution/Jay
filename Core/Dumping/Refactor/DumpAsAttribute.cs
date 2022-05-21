namespace Jay.Dumping.Refactor;

[AttributeUsage(AttributeTargets.Enum)]
public class DumpAsAttribute : Attribute, IDumpable
{
    internal string? Value { get; }

    public DumpAsAttribute(char ch)
    {
        if (ch == default)
        {
            Value = default;
        }
        else
        {
            Value = new string(ch, 1);
        }
    }

    public DumpAsAttribute(string? dump)
    {
        if (string.IsNullOrWhiteSpace(dump))
        {
            Value = default;
        }
        else
        {
            Value = dump;
        }
    }

    public void Dump(ref Dumper dumper)
    {
        dumper.AppendLiteral(Value);
    }

    public override string ToString()
    {
        return $"Dump as '{Value}'";
    }
}