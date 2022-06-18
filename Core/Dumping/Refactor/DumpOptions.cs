namespace Jay.Dumping.Refactor;

public class DumpOptions
{
    public static DumpOptions Default { get; } = new DumpOptions();

    public string? Format { get; init; } = null;
    public IFormatProvider? FormatProvider { get; init; } = null;
    public bool Verbose { get; init; } = false;
    public int? WrapLength { get; init; } = null;
}