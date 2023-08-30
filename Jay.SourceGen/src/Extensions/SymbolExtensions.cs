namespace Jay.SourceGen.Extensions;

public static class SymbolExtensions
{
    public static string? GetNamespace(this ISymbol typeSymbol)
    {
        var nsSymbol = typeSymbol.ContainingNamespace;
        string? nameSpace;
        if (nsSymbol.IsGlobalNamespace)
        {
            //nameSpace = nsSymbol.ContainingModule.ContainingSymbol.Name;
            nameSpace = null;
        }
        else
        {
            nameSpace = nsSymbol.Name;
        }
        return nameSpace;
    }

    public static string GetFullName(this ISymbol typeSymbol)
    {
        var symbolDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        return typeSymbol.ToDisplayString(symbolDisplayFormat);
    }

    public static string GetFQNamespace(this ISymbol typeSymbol)
    {
        var nsSymbol = typeSymbol.ContainingNamespace;
        var ns = nsSymbol.ToString();
        return ns;
    }

    public static MemberTypes GetMemberType(this ISymbol? symbol)
    {
        return symbol switch
        {
            IFieldSymbol => MemberTypes.Field,
            IPropertySymbol => MemberTypes.Property,
            IEventSymbol => MemberTypes.Event,
            IMethodSymbol => MemberTypes.Method,
            _ => default,
        };
    }
}
