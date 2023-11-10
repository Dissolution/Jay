namespace Jay.SourceGen.Reflection;

public record class TypeSignature : MemberSignature
{
    public static TypeSignature? Create(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return null;
        TypeSignature signature = new()
        {
            Name = typeSymbol.ToString(),
            Visibility = typeSymbol.GetVisibility(),
            Keywords = typeSymbol.GetKeywords(),
            Namespace = typeSymbol.GetNamespace(),
            Kind = typeSymbol.TypeKind,
            BaseType = Create(typeSymbol.BaseType),
            Attributes = Attributes.From(typeSymbol),
        };
        return signature;
    }
    
    public static TypeSignature? Create(Type? type)
    {
        if (type is null) return null;
        TypeSignature signature = new()
        {
            Name = type.ToString(),
            Visibility = type.GetVisibility(),
            Keywords = type.GetKeywords(),
            Namespace = type.Namespace,
            Kind = type.TypeKind(),
            BaseType = Create(type.BaseType),
            Attributes = Attributes.From(type),
        };
        return signature;
    }
    
    
    public string? Namespace { get; set; } = null;
    public TypeKind Kind { get; set; } = TypeKind.Unknown;

    public string FullyQualifiedName => $"{Namespace}.{Name}";

    public override bool WriteTo(CodeBuilder codeBuilder)
    {


        return codeBuilder.Wrote(cb => cb.Append(Name));
    }
}