using Jay.SourceGen.Collections;

namespace Jay.SourceGen.Reflection;

public sealed class AttributeSignature : Signature,
        IEquatable<AttributeSignature>,
        IEquatable<AttributeData>,
        IEquatable<CustomAttributeData>
{
    [return: NotNullIfNotNull(nameof(attributeData))]
    public static implicit operator AttributeSignature?(AttributeData? attributeData) =>
        Create(attributeData);

    [return: NotNullIfNotNull(nameof(customAttrData))]
    public static implicit operator AttributeSignature?(CustomAttributeData? customAttrData) =>
        Create(customAttrData);

    public static bool operator ==(AttributeSignature? left, AttributeSignature? right) => FastEqual(left, right);
    public static bool operator !=(AttributeSignature? left, AttributeSignature? right) => !FastEqual(left, right);
    public static bool operator ==(AttributeSignature? left, AttributeData? right) => FastEquality(left, right);
    public static bool operator !=(AttributeSignature? left, AttributeData? right) => !FastEquality(left, right);
    public static bool operator ==(AttributeSignature? left, CustomAttributeData? right) => FastEquality(left, right);
    public static bool operator !=(AttributeSignature? left, CustomAttributeData? right) => !FastEquality(left, right);

    [return: NotNullIfNotNull(nameof(attributeData))]
    public static AttributeSignature? Create(AttributeData? attributeData)
    {
        if (attributeData is null)
            return null;
        return new AttributeSignature(attributeData);
    }

    [return: NotNullIfNotNull(nameof(customAttrData))]
    public static AttributeSignature? Create(CustomAttributeData? customAttrData)
    {
        if (customAttrData is null)
            return null;
        return new AttributeSignature(customAttrData);
    }
    
    public TypeSignature? AttributeType { get; }
    public AttributeArguments Arguments { get; }

    public AttributeSignature(AttributeData attributeData)
        : base(attributeData.AttributeClass?.Name ?? attributeData.ToString(), default, default)
    {
        this.AttributeType = TypeSignature.Create(attributeData.AttributeClass);
        this.Arguments = new(attributeData);
    }

    public AttributeSignature(CustomAttributeData customAttributeData)
        : base(customAttributeData.AttributeType.Name, default, default)
    {
        this.AttributeType = TypeSignature.Create(customAttributeData.AttributeType);
        this.Arguments = new(customAttributeData);
    }

    public override bool Equals(Signature? signature)
    {
        return signature is AttributeSignature attributeSig && Equals(attributeSig);
    }
    
    public bool Equals(AttributeSignature? attributeSig)
    {
        return attributeSig is not null
            && FastEqual(AttributeType, attributeSig.AttributeType)
            && TextEqual(Name, attributeSig.Name)
            && SetEqual(Arguments, attributeSig.Arguments);
    }

    public bool Equals(AttributeData? attributeData) => Equals(Create(attributeData));

    public bool Equals(CustomAttributeData? customAttrData) => Equals(Create(customAttrData));
    
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            AttributeSignature attributeSig => Equals(attributeSig),
            AttributeData attributeData => Equals(attributeData),
            CustomAttributeData customAttrData => Equals(customAttrData),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(AttributeType, Name, Arguments);
    }

    public override void DeclareTo(CodeBuilder code)
    {
        code.Write(Name);
        if (Arguments.Count > 0)
        {
            code.Append('(')
                .Delimit(static c => c.Write(", "), Arguments, static (cb, a) => cb.Append(a.Key).Append(" = ").Append(a.Value))
                .Write(')');
        }
    }
}
