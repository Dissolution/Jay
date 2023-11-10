namespace Jay.SourceGen.Reflection;

public record class FieldSignature : MemberSignature
{
    public static FieldSignature? Create(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol is null) return null;
        return new FieldSignature()
        {
            Name = fieldSymbol.Name,
            Visibility = fieldSymbol.GetVisibility(),
            Keywords = fieldSymbol.GetKeywords(),
            Attributes = Attributes.From(fieldSymbol),
            ValueType = TypeSignature.Create(fieldSymbol.Type),
        };
    }
    
    public static FieldSignature? Create(FieldInfo? fieldInfo)
    {
        if (fieldInfo is null) return null;
        return new FieldSignature()
        {
            Name = fieldInfo.Name,
            Visibility = fieldInfo.GetVisibility(),
            Keywords = fieldInfo.GetKeywords(),
            Attributes = Attributes.From(fieldInfo),
            ValueType = TypeSignature.Create(fieldInfo.FieldType),
        };
    }
    
    
    public TypeSignature? ValueType { get; set; } = null;

    public override bool WriteTo(CodeBuilder codeBuilder)
    {
        return codeBuilder.Wrote(cb => cb
            .IfAppend(Visibility, ' ')
            .IfAppend(Keywords, ' ')
            .IfAppend(ValueType, ' ')
            .Append(Name));
    }
}