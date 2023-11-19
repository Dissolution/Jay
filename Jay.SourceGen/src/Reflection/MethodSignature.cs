namespace Jay.SourceGen.Reflection;

public record class MethodSignature : MemberSignature
{
    public static MethodSignature? Create(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol is null) return null;
        return new MethodSignature()
        {
            Name = methodSymbol.Name,
            Visibility = methodSymbol.GetVisibility(),
            Keywords = methodSymbol.GetKeywords(),
            Attributes = Attributes.From(methodSymbol),
            Parameters = Parameters.From(methodSymbol.Parameters),
            ReturnType = TypeSignature.Create(methodSymbol.ReturnType),
        };
    }
    
    public static MethodSignature? Create(MethodBase? methodBase)
    {
        if (methodBase is null) return null;
        return new MethodSignature()
        {
            Name = methodBase.Name,
            Visibility = methodBase.Visibility(),
            Keywords = methodBase.GetKeywords(),
            Attributes = Attributes.From(methodBase),
            Parameters = Parameters.From(methodBase.GetParameters()),
            ReturnType = TypeSignature.Create(methodBase.ReturnType()),
        };
    }

    public Parameters Parameters { get; set; } = new();
    public TypeSignature? ReturnType { get; set; } = null;
}