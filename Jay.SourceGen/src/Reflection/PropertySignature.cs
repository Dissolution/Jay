namespace Jay.SourceGen.Reflection;

public record class PropertySignature : MemberSignature
{
    public static PropertySignature? Create(IPropertySymbol? propertySymbol)
    {
        if (propertySymbol is null) return null;
        return new PropertySignature()
        {
            Name = propertySymbol.Name,
            Visibility = propertySymbol.GetVisibility(),
            Keywords = propertySymbol.GetKeywords(),
            Attributes = Attributes.From(propertySymbol),
            ValueType = TypeSignature.Create(propertySymbol.Type),
            Getter = MethodSignature.Create(propertySymbol.GetMethod),
            Setter = MethodSignature.Create(propertySymbol.SetMethod),
            IsIndexer = propertySymbol.IsIndexer,
            Parameters = Parameters.From(propertySymbol.Parameters),
        };
    }
    
    public static PropertySignature? Create(PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null) return null;
        return new PropertySignature()
        {
            Name = propertyInfo.Name,
            Visibility = propertyInfo.Visibility(),
            Keywords = propertyInfo.GetKeywords(),
            Attributes = Attributes.From(propertyInfo),
            ValueType = TypeSignature.Create(propertyInfo.PropertyType),
            Getter = MethodSignature.Create(propertyInfo.GetMethod),
            Setter = MethodSignature.Create(propertyInfo.SetMethod),
            IsIndexer = propertyInfo.GetIndexParameters().Length > 0,
            Parameters = Parameters.From(propertyInfo.GetIndexParameters()),
        };
    }

    public FieldSignature? BackingField { get; set; } = null;
    public TypeSignature? ValueType { get; set; } = null;
    public MethodSignature? Getter { get; set; } = null;
    public MethodSignature? Setter { get; set; } = null;
    public bool IsIndexer { get; set; } = false;
    public Parameters Parameters { get; set; } = new();
}