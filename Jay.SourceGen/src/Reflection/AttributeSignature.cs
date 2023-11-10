namespace Jay.SourceGen.Reflection;

public record class AttributeSignature : Signature
{
    public static AttributeSignature? Create(AttributeData? attributeData)
    {
        if (attributeData is null) return null;
        var signature = new AttributeSignature
        {
            Name = attributeData.AttributeClass?.Name ?? attributeData.ToString(),
            Visibility = Visibility.Public | Visibility.Static,
            Arguments = AttributeArguments.From(attributeData),
        };
        return signature;
    }

    public static AttributeSignature? Create(CustomAttributeData? customAttributeData)
    {
        if (customAttributeData is null) return null;
        var signature = new AttributeSignature
        {
            Name = customAttributeData.AttributeType.Name,
            Visibility = Visibility.Public | Visibility.Static,
            Arguments = AttributeArguments.From(customAttributeData),
        };
        return signature;
    }
    
    
    public AttributeArguments Arguments { get; set; } = new();
}