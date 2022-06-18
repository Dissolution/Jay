using Jay.Text;

namespace Jay.Dumping.Refactor;

[AttributeUsage(AttributeTargets.Enum)]
public class DumpAsAttribute : Attribute
{
    public string? DumpString { get; }

    public DumpAsAttribute(char ch)
    {
        if (ch == default)
        {
            DumpString = default;
        }
        else
        {
            DumpString = new string(ch, 1);
        }
    }

    public DumpAsAttribute(string? dump)
    {
        if (string.IsNullOrWhiteSpace(dump))
        {
            DumpString = default;
        }
        else
        {
            DumpString = dump;
        }
    }

    public override string ToString()
    {
        return $"Dump as '{DumpString}'";
    }
}