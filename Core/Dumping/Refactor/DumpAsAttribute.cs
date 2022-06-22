namespace Jay.Dumping.Refactor2;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class DumpAsAttribute : Attribute
{
    internal string DumpAs { get; }

    public DumpAsAttribute(char ch)
    {
        if (ch == default)
            throw new ArgumentException("Cannot dump as '\\0'", nameof(ch));
        DumpAs = new string(ch, 1);
    }

    public DumpAsAttribute(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException("Cannot dump as null or whitespace", nameof(str));
        DumpAs = str;
    }

    public override string ToString() => DumpAs;
}