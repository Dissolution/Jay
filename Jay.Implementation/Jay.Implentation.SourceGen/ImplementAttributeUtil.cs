using System.Diagnostics;

namespace IMPL.SourceGen;

internal static class ImplementAttributeUtil
{
    public static readonly string FullName = typeof(ImplementAttribute).FullName;
    public static readonly string Declaration_PropertyName = nameof(ImplementAttribute.Declaration);
    public static readonly string Name_PropertyName = nameof(ImplementAttribute.Name);

    private static bool TryParseDeclarationSegment(string segment, out Typewords typewords)
    {
        var seg = segment.Trim();

        typewords = default;
        if (Enum.TryParse<Visibility>(seg, true, out var visibility))
            typewords.Visibility |= visibility;
        else if (Enum.TryParse<Instic>(seg, true, out var instic))
            typewords.Instic |= instic;
        else if (Enum.TryParse<Keywords>(seg, true, out var keywords))
            typewords.Keywords |= keywords;
        else if (Enum.TryParse<ObjType>(seg, true, out var objType))
            typewords.ObjType |= objType;
        else
        {
            Debugger.Break();
            return false;
        }
        return true;
    }

    public static bool TryParseDeclaration(string? declaration, out Typewords typewords)
    {
        typewords = default;
        if (string.IsNullOrEmpty(declaration)) return false;
        if (TryParseDeclarationSegment(declaration!, out typewords))
            return true;
        var split = declaration!.Split(new char[]{' ', ',', '|'}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in split)
        {
            if (TryParseDeclarationSegment(segment, out var segmentWords))
            {
                typewords |= segmentWords;
            }
            else
            {
                Debugger.Break();
                typewords = default;
                return false;
            }
        }
        return true;
    }

}