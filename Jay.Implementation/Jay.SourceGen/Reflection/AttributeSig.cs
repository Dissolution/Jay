namespace Jay.SourceGen.Reflection;

public sealed class AttributeSig : Sig,
        IEquatable<AttributeSig>,
        IEquatable<AttributeData>,
        IEquatable<CustomAttributeData>
{
    [return: NotNullIfNotNull(nameof(attributeData))]
    public static implicit operator AttributeSig?(AttributeData? attributeData) =>
        AttributeSig.Create(attributeData);

    [return: NotNullIfNotNull(nameof(customAttrData))]
    public static implicit operator AttributeSig?(CustomAttributeData? customAttrData) =>
        AttributeSig.Create(customAttrData);

    public static bool operator ==(AttributeSig? left, AttributeSig? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(AttributeSig? left, AttributeSig? right)
    {
        if (left is null)
            return right is not null;
        return !left.Equals(right);
    }

    public static bool operator ==(AttributeSig? left, AttributeData? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(AttributeSig? left, AttributeData? right)
    {
        if (left is null)
            return right is not null;
        return !left.Equals(right);
    }

    public static bool operator ==(AttributeSig? left, CustomAttributeData? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(AttributeSig? left, CustomAttributeData? right)
    {
        if (left is null)
            return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(attributeData))]
    public static AttributeSig? Create(AttributeData? attributeData)
    {
        if (attributeData is null)
            return null;
        return new AttributeSig(attributeData);
    }

    [return: NotNullIfNotNull(nameof(customAttrData))]
    public static AttributeSig? Create(CustomAttributeData? customAttrData)
    {
        if (customAttrData is null)
            return null;
        return new AttributeSig(customAttrData);
    }

    public AttributeArgsDictionary Args { get; }

    public AttributeSig()
        : base(SigType.Attribute)
    {
        this.Args = new();
    }

    public AttributeSig(AttributeData attributeData)
        : base(SigType.Attribute)
    {
        var attrClass = attributeData.AttributeClass;
        this.Name = attrClass?.Name ?? attributeData.ToString();
        this.Args = new(attributeData);
    }

    public AttributeSig(CustomAttributeData customAttributeData)
        : base(SigType.Attribute)
    {
        this.Name = customAttributeData.AttributeType.Name;
        this.Args = new(customAttributeData);
    }

    public bool Equals(AttributeSig? attributeSig)
    {
        return attributeSig is not null
            && this.Name == attributeSig.Name
            && this.Args.SequenceEqual(attributeSig.Args);
    }

    public bool Equals(AttributeData? attributeData)
    {
        return attributeData is not null
            && this.Name == attributeData.AttributeClass?.Name
            && this.Args.SequenceEqual(attributeData.GetArgs());
    }

    public bool Equals(CustomAttributeData? customAttrData)
    {
        return customAttrData is not null
            && this.Name == customAttrData.AttributeType.Name
            && this.Args.SequenceEqual(customAttrData.GetArgs());
    }

    public override bool Equals(Sig? signature)
    {
        return signature is AttributeSig attributeSig && Equals(attributeSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is AttributeSig attributeSig)
            return Equals(attributeSig);
        if (obj is AttributeData attributeData)
            return Equals(attributeData);
        if (obj is CustomAttributeData customAttrData)
            return Equals(customAttrData);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Attribute, Name, Args);
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .Append('[')
            .Append(Name)
            .If(Args.Count > 0,
                cb => cb.Append('(')
                        .Delimit(", ", Args, static (ab, a) => ab.Code($"{a.Key} = {a.Value}"))
                        .Append(')')
            )
            .Append(']')
            .ToStringAndDispose();
    }
}
