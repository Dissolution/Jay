namespace Jay.Dumping;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DumpAsAttribute : Attribute
{
    internal string DumpAs { get; }

    public DumpAsAttribute(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            str = string.Empty;
        DumpAs = str;
    }

    public override string ToString() => DumpAs;
}