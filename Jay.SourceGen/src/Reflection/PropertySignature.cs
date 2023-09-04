namespace Jay.SourceGen.Reflection;

public sealed class PropertySignature : MemberSignature,
    IEquatable<PropertySignature>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
{
    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static implicit operator PropertySignature?(PropertyInfo? propertyInfo) => Create(propertyInfo);

    public static bool operator ==(PropertySignature? left, PropertySignature? right) => FastEqual(left, right);
    public static bool operator !=(PropertySignature? left, PropertySignature? right) => !FastEqual(left, right);
    public static bool operator ==(PropertySignature? left, IPropertySymbol? right) => FastEquality(left, right);
    public static bool operator !=(PropertySignature? left, IPropertySymbol? right) => !FastEquality(left, right);
    public static bool operator ==(PropertySignature? left, PropertyInfo? right) => FastEquality(left, right);
    public static bool operator !=(PropertySignature? left, PropertyInfo? right) => !FastEquality(left, right);

    [return: NotNullIfNotNull(nameof(propertySymbol))]
    public static PropertySignature? Create(IPropertySymbol? propertySymbol)
    {
        if (propertySymbol is null)
            return null;

        return new PropertySignature(propertySymbol);
    }

    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static PropertySignature? Create(PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return null;

        return new PropertySignature(propertyInfo);
    }

    public TypeSignature? PropertyType { get; init; }
    public MethodSignature? GetMethod { get; init; }
    public MethodSignature? SetMethod { get; init; }
    public bool IsIndexer { get; init; }
    public IReadOnlyList<ParameterSignature> Parameters { get; init; }

    public bool HasGetter => GetMethod is not null;
    public bool HasSetter => SetMethod is not null;

    public PropertySignature(IPropertySymbol propertySymbol)
        : base(propertySymbol)
    {
        this.PropertyType = new(propertySymbol.Type);
        this.GetMethod = MethodSignature.Create(propertySymbol.GetMethod);
        this.SetMethod = MethodSignature.Create(propertySymbol.SetMethod);
        this.IsIndexer = propertySymbol.IsIndexer;
        this.Parameters = propertySymbol.Parameters.Select(static p => new ParameterSignature(p))
            .ToList();
    }

    public PropertySignature(PropertyInfo propertyInfo)
        : base(propertyInfo)
    {
        this.PropertyType = new(propertyInfo.PropertyType);
        this.GetMethod = MethodSignature.Create(propertyInfo.GetMethod);
        this.SetMethod = MethodSignature.Create(propertyInfo.SetMethod);
        this.Parameters = propertyInfo.GetIndexParameters()
            .Select(static p => new ParameterSignature(p))
            .ToList();
        this.IsIndexer = Parameters.Count > 0;
    }

    public override bool Equals(Signature? signature)
    {
        return signature is PropertySignature propertySig && Equals(propertySig);
    }

    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is PropertySignature propertySig && Equals(propertySig);
    }

    public bool Equals(PropertySignature? propertySig)
    {
        return base.Equals(propertySig) 
            && FastEqual(PropertyType, propertySig.PropertyType) 
            && FastEqual(GetMethod, propertySig.GetMethod) 
            && FastEqual(SetMethod, propertySig.SetMethod);
    }

    public bool Equals(IPropertySymbol? propertySymbol) => Equals(Create(propertySymbol));

    public bool Equals(PropertyInfo? propertyInfo) => Equals(Create(propertyInfo));
    
    public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));

    public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            PropertySignature propertySig => Equals(propertySig),
            IPropertySymbol propertySymbol => Equals(propertySymbol),
            PropertyInfo propertyInfo => Equals(propertyInfo),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(base.GetHashCode(), PropertyType, GetMethod, SetMethod);
    }

    public override void DeclareTo(SourceCodeBuilder code)
    {
        code.Code(Attributes)
            .Code(Visibility)
            .Write(' ')
            .Code(Keywords)
            .Write(' ')
            .Code(PropertyType)
            .Write(' ')
            .Write(Name);
        if (!HasGetter && !HasSetter)
        {
            code.Write(';');
        }
        else
        {
            code.Write("{ ");
            var getter = GetMethod;
            if (getter is not null)
            {
                if (getter.Visibility != Visibility)
                {
                    code.Code(getter.Visibility)
                        .Write(' ');
                }
                code.Write("get; ");
            }
            var setter = SetMethod;
            if (setter is not null)
            {
                if (setter.Visibility != Visibility)
                {
                    code.Code(setter.Visibility)
                        .Write(' ');
                }
                if (setter.Keywords.HasFlags(Keywords.Init))
                {
                    code.Write("init; ");
                }
                else
                {
                    code.Write("set; ");
                }
            }
            code.Write('}');
        }
    }
}