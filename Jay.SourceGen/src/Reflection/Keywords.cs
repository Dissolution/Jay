namespace Jay.SourceGen.Reflection;

[Flags]
public enum Keywords
{
    New = 1 << 1,
    Const = 1 << 2,
    Abstract = 1 << 3,
    Virtual = 1 << 4,
    Sealed = 1 << 5,
    Readonly = 1 << 6,
    Override = 1 << 7,
    Extern = 1 << 8,
    Unsafe = 1 << 9,
    Volatile = 1 << 10,
    Async = 1 << 11,
    Required = 1 << 12,
    Init = 1 << 13,
}

public static class KeywordExtensions
{
    public static Keywords GetKeywords(this MemberInfo? member)
    {
        Keywords keywords = default;
        switch (member)
        {
            case null:
                return keywords;
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsInitOnly)
                    keywords.AddFlag(Keywords.Readonly);
                if (fieldInfo.IsLiteral)
                    keywords.AddFlag(Keywords.Const);
                return keywords;
            }
            case MethodBase method:
            {
                if (method.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                if (method.IsFinal)
                    keywords.AddFlag(Keywords.Sealed);
                if (method.IsVirtual)
                    keywords.AddFlag(Keywords.Virtual);
                return keywords;
            }
            case PropertyInfo propertyInfo:
            {
                keywords.AddFlag(GetKeywords(propertyInfo.GetGetter()));
                keywords.AddFlag(GetKeywords(propertyInfo.GetSetter()));
                return keywords;
            }
            case EventInfo eventInfo:
            {
                keywords.AddFlag(GetKeywords(eventInfo.GetAdder()));
                keywords.AddFlag(GetKeywords(eventInfo.GetRemover()));
                keywords.AddFlag(GetKeywords(eventInfo.GetRaiser()));
                return keywords;
            }
            case Type type:
            {
                if (type.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                if (type.IsSealed)
                    keywords.AddFlag(Keywords.Sealed);
                return keywords;
            }
            default:
                return keywords;
        }
    }

    public static Keywords GetKeywords(this ISymbol? symbol)
    {
        Keywords keywords = default;
         if (symbol is null)
             return keywords;
        if (symbol.IsAbstract)
            keywords.AddFlag(Keywords.Abstract);
        if (symbol.IsSealed)
            keywords.AddFlag(Keywords.Sealed);
        if (symbol.IsVirtual)
            keywords.AddFlag(Keywords.Virtual);
        if (symbol.IsExtern)
            keywords.AddFlag(Keywords.Extern);
        if (symbol.IsOverride)
            keywords.AddFlag(Keywords.Override);
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
            {
                if (fieldSymbol.IsConst)
                    keywords.AddFlag(Keywords.Const);
                if (fieldSymbol.IsRequired)
                    keywords.AddFlag(Keywords.Required);
                if (fieldSymbol.IsVolatile)
                    keywords.AddFlag(Keywords.Volatile);
                if (fieldSymbol.IsReadOnly)
                    keywords.AddFlag(Keywords.Readonly);
                return keywords;
            }
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
            case IEventSymbol eventSymbol:
            {
                keywords.AddFlag(GetKeywords(eventSymbol.AddMethod));
                keywords.AddFlag(GetKeywords(eventSymbol.RemoveMethod));
                keywords.AddFlag(GetKeywords(eventSymbol.RaiseMethod));
                return keywords;
            }
            case IPropertySymbol propertySymbol:
            {
                keywords.AddFlag(GetKeywords(propertySymbol.GetMethod));
                keywords.AddFlag(GetKeywords(propertySymbol.SetMethod));
                return keywords;
            }
            case ITypeSymbol typeSymbol:
            {
                if (typeSymbol.IsReadOnly)
                    keywords.AddFlag(Keywords.Readonly);
                return keywords;
            }
            default:
                return keywords;
        }
    }
}