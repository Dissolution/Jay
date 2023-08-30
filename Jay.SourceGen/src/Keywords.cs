using System.Runtime.CompilerServices;
using Jay.Enums;
using Jay.Reflection.Caching;

namespace Jay.SourceGen.Scratch;

[Flags]
public enum Keywords
{
    None = 0,
    
    Override,
    
    // Field
    Const,
    Readonly,
    
    // Property
    Required,
    
    // Event
    
    // Method
    Init,
    Virtual,
    Abstract,
    Sealed,
    Async,
}

public static class KeywordsExtensions
{
    public static void WriteCodeTo(this Keywords keywords, CodeBuilder codeBuilder)
    {
        codeBuilder.Enumerate(
            keywords.GetFlags(),
            static (cb, k) =>
            {
                var written = cb.GetWrittenSpan(c => c.Format<Keywords>(k));
                written[0] = char.ToLower(written[0]);
            });
    }
    
    
    public static Keywords GetKeywords(this MemberInfo? member)
    {
        var keywords = Keywords.None;
        switch (member)
        {
            case null:
                return keywords;
            case FieldInfo field:
            {
                if (field.IsInitOnly)
                    keywords.AddFlag(Keywords.Readonly);
                if (field.IsLiteral)
                    keywords.AddFlag(Keywords.Const);
                return keywords;
            }
            case PropertyInfo property:
            {
                keywords.AddFlag(GetKeywords(property.GetMethod));
                keywords.AddFlag(GetKeywords(property.SetMethod));
                if (property.GetCustomAttribute<RequiredMemberAttribute>() != null)
                {
                    keywords.AddFlag(Keywords.Required);
                }
                return keywords;
            }
            case EventInfo eventInfo:
            {
                keywords.AddFlag(GetKeywords(eventInfo.AddMethod));
                keywords.AddFlag(GetKeywords(eventInfo.RemoveMethod));
                keywords.AddFlag(GetKeywords(eventInfo.RaiseMethod));
                return keywords;
            }
            case MethodBase method:
            {
                if (method.IsVirtual)
                    keywords.AddFlag(Keywords.Virtual);
                if (method.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                if (method.IsFinal || !method.IsVirtual)
                    keywords.AddFlag(Keywords.Sealed);

                // Look for init for property set methods
                if (method is MethodInfo methodInfo)
                {
                    var returnMods = methodInfo.ReturnParameter!.GetRequiredCustomModifiers();
                    if (returnMods.Contains(typeof(IsExternalInit)))
                    {
                        keywords.AddFlag(Keywords.Init);
                    }
                }
                return keywords;
            }
            case Type type:
            {
                if (type.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                return keywords;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
    }
    
    public static Keywords GetKeywords(this ISymbol? symbol)
    {
        Keywords keywords = default;
        if (symbol is null) 
            return keywords;
        if (symbol.IsVirtual)
            keywords.AddFlag(Keywords.Virtual);
        if (symbol.IsAbstract)
            keywords.AddFlag(Keywords.Abstract);
        if (symbol.IsSealed)
            keywords.AddFlag(Keywords.Sealed);
        if (symbol.IsOverride)
            keywords.AddFlag(Keywords.Override);
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
            {
                if (fieldSymbol.IsReadOnly)
                    keywords.AddFlag(Keywords.Readonly);
                return keywords;
            }
            case IPropertySymbol propertySymbol:
            {
                if (propertySymbol.IsReadOnly)
                    keywords |= Keywords.Readonly;
                if (propertySymbol.IsRequired)
                    keywords |= Keywords.Required;
                keywords.AddFlag(GetKeywords(propertySymbol.GetMethod));
                keywords.AddFlag(GetKeywords(propertySymbol.SetMethod));
                return keywords;
            }
            case IEventSymbol eventSymbol:
                keywords.AddFlag(GetKeywords(eventSymbol.AddMethod));
                keywords.AddFlag(GetKeywords(eventSymbol.RemoveMethod));
                keywords.AddFlag(GetKeywords(eventSymbol.RaiseMethod));
                return keywords;
            case IMethodSymbol methodSymbol:
            {
                if (methodSymbol.IsInitOnly)
                    keywords.AddFlag(Keywords.Init);
                if (methodSymbol.IsReadOnly)
                    keywords.AddFlag(Keywords.Readonly);
                if (methodSymbol.IsAsync)
                    keywords.AddFlag(Keywords.Async);
                return keywords;
            }
            case ITypeSymbol typeSymbol:
            {
                if (typeSymbol.IsReadOnly)
                    keywords.AddFlag(Keywords.Readonly);
                return keywords;
            }
            default:
                throw new ArgumentException("", nameof(symbol));
        }
    }

}