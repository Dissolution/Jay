using Jay.SourceGen.Utilities;

namespace Jay.SourceGen.Reflection;

[Flags]
public enum Keywords : ulong
{
    None = 0,
    Abstract = 1 << 0,
    Sealed = 1 << 1,
    Virtual = 1 << 2,
    Extern = 1 << 3,
    Override = 1 << 4,
    Const = 1 << 5,
    Required = 1 << 6,
    Volatile = 1 << 7,
    Readonly = 1 << 8,
    Init = 1 << 9,
    Async = 1 << 10,
}

public sealed class KeywordsToCode : ToCodeProvider<Keywords>
{
    public override bool WriteTo(Keywords keywords, CodeBuilder codeBuilder)
    {
        var flags = keywords.GetFlags();
        if (flags.Count == 0) return false;
        codeBuilder.Delimit(" ", keywords.GetFlags(), static (cb, k) => cb.Append(k, Casing.Lower));
        return true;
    }
}