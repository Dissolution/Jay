using Jay.Enums;

namespace Jay.SourceGen.Reflection;



public sealed class KeywordsToCode : ToCodeProvider<Keywords>
{
    public override bool WriteTo(Keywords keywords, CodeBuilder codeBuilder)
    {
        var flags = keywords.GetFlags();
        if (flags.Length == 0) return false;
        codeBuilder.Delimit(" ", keywords.GetFlags(), static (cb, k) => cb.Append(k, Casing.Lower));
        return true;
    }
}