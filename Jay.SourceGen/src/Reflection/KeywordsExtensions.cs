namespace Jay.SourceGen.Reflection;

public static class KeywordsExtensions
{
    public static Keywords GetKeywords(this MemberInfo? member)
    {
        Keywords keywords = Keywords.None;
        switch (member)
        {
            case null:
                return keywords;
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsInitOnly)
                    keywords |= Keywords.Readonly;
                if (fieldInfo.IsLiteral)
                    keywords |= Keywords.Const;
                return keywords;
            }
            case MethodBase method:
            {
                if (method.IsAbstract)
                    keywords |= Keywords.Abstract;
                if (method.IsFinal)
                    keywords |= Keywords.Sealed;
                if (method.IsVirtual)
                    keywords |= Keywords.Virtual;
                return keywords;
            }
            case PropertyInfo propertyInfo:
            {
                keywords |= GetKeywords(propertyInfo.GetMethod);
                keywords |= GetKeywords(propertyInfo.SetMethod);
                return keywords;
            }
            case EventInfo eventInfo:
            {
                keywords |= GetKeywords(eventInfo.AddMethod);
                keywords |= GetKeywords(eventInfo.RemoveMethod);
                keywords |= GetKeywords(eventInfo.RaiseMethod);
                return keywords;
            }
            case Type type:
            {
                if (type.IsAbstract)
                    keywords |= Keywords.Abstract;
                if (type.IsSealed)
                    keywords |= Keywords.Sealed;
                return keywords;
            }
            default:
                return keywords;
        }
    }

    public static Keywords GetKeywords(this ISymbol? symbol)
    {
        Keywords keywords = Keywords.None;
        if (symbol is null) return keywords;

        if (symbol.IsAbstract)
            keywords |= Keywords.Abstract;
        if (symbol.IsSealed)
            keywords |= Keywords.Sealed;
        if (symbol.IsVirtual)
            keywords |= Keywords.Virtual;
        if (symbol.IsExtern)
            keywords |= Keywords.Extern;
        if (symbol.IsOverride)
            keywords |= Keywords.Override;
        
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
            {
                if (fieldSymbol.IsConst)
                    keywords |= Keywords.Const;
                if (fieldSymbol.IsRequired)
                    keywords |= Keywords.Required;
                if (fieldSymbol.IsVolatile)
                    keywords |= Keywords.Volatile;
                if (fieldSymbol.IsReadOnly)
                    keywords |= Keywords.Readonly;
                break;
            }
            case IMethodSymbol methodSymbol:
            {
                if (methodSymbol.IsInitOnly)
                    keywords |= Keywords.Init;
                if (methodSymbol.IsReadOnly)
                    keywords |= Keywords.Readonly;
                if (methodSymbol.IsAsync)
                    keywords |= Keywords.Async;
                break;
            }
            case IEventSymbol eventSymbol:
            {
                keywords |= GetKeywords(eventSymbol.AddMethod);
                keywords |= GetKeywords(eventSymbol.RemoveMethod);
                keywords |= GetKeywords(eventSymbol.RaiseMethod);
                break;
            }
            case IPropertySymbol propertySymbol:
            {
                keywords |= GetKeywords(propertySymbol.GetMethod);
                keywords |= GetKeywords(propertySymbol.SetMethod);
                break;
            }
            case ITypeSymbol typeSymbol:
            {
                if (typeSymbol.IsReadOnly)
                    keywords |= Keywords.Readonly;
                break;
            }
        }

        return keywords;
    }
}