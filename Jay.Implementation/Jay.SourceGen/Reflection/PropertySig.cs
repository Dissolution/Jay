namespace Jay.SourceGen.Reflection;

public sealed class PropertySig : MemberSig,
    IEquatable<PropertySig>, IEquatable<IPropertySymbol>, IEquatable<PropertyInfo>
{
    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static implicit operator PropertySig?(PropertyInfo? propertyInfo) => PropertySig.Create(propertyInfo);


    public static bool operator ==(PropertySig? left, PropertySig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, PropertySig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(PropertySig? left, IPropertySymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, IPropertySymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(PropertySig? left, PropertyInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(PropertySig? left, PropertyInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(propertySymbol))]
    public static PropertySig? Create(IPropertySymbol? propertySymbol)
    {
        if (propertySymbol is null) return null;
        return new PropertySig(propertySymbol);
    }

    [return: NotNullIfNotNull(nameof(propertyInfo))]
    public static PropertySig? Create(PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null) return null;
        return new PropertySig(propertyInfo);
    }

    public TypeSig? PropertyType { get; set; } = null;
    public MethodSig? GetMethod { get; set; } = null;
    public MethodSig? SetMethod { get; set; } = null;
    public bool IsIndexer { get; set; } = false;
    public IReadOnlyList<ParameterSig> Parameters { get; set; } = Array.Empty<ParameterSig>();

    public bool HasGetter => GetMethod is not null;
    public bool HasSetter => SetMethod is not null;

    public PropertySig()
        : base(SigType.Property)
    {

    }

    public PropertySig(IPropertySymbol propertySymbol)
        : base(SigType.Property, propertySymbol)
    {
        this.PropertyType = TypeSig.Create(propertySymbol.Type);
        this.GetMethod = MethodSig.Create(propertySymbol.GetMethod);
        this.SetMethod = MethodSig.Create(propertySymbol.SetMethod);
        this.IsIndexer = propertySymbol.IsIndexer;
        this.Parameters = propertySymbol.Parameters.Select(static p => new ParameterSig(p)).ToList();
    }

    public PropertySig(PropertyInfo propertyInfo)
        : base(SigType.Property, propertyInfo)
    {
        this.PropertyType = TypeSig.Create(propertyInfo.PropertyType);
        this.GetMethod = MethodSig.Create(propertyInfo.GetMethod);
        this.SetMethod = MethodSig.Create(propertyInfo.SetMethod);
        this.Parameters = propertyInfo.GetIndexParameters().Select(static p => new ParameterSig(p)).ToList();
        this.IsIndexer = this.Parameters.Count > 0;
    }


    public bool Equals(PropertySig? propertySig)
    {
        return propertySig is not null &&
            this.Name == propertySig.Name &&
            this.PropertyType == propertySig.PropertyType &&
            this.HasGetter == propertySig.HasGetter &&
            this.HasSetter == propertySig.HasSetter;
    }

    public bool Equals(IPropertySymbol? propertySymbol)
    {
        return propertySymbol is not null &&
            this.Name == propertySymbol.Name &&
            this.PropertyType == propertySymbol.Type &&
            this.HasGetter == propertySymbol.GetMethod is not null &&
            this.HasSetter == propertySymbol.SetMethod is not null;
    }

    public bool Equals(PropertyInfo? propertyInfo)
    {
        return propertyInfo is not null &&
            this.Name == propertyInfo.Name &&
            this.PropertyType == propertyInfo.PropertyType &&
            this.HasGetter == propertyInfo.GetMethod is not null &&
            this.HasSetter == propertyInfo.SetMethod is not null;
    }

      public override bool Equals(Sig? signature)
    {
        return signature is PropertySig propertySig && Equals(propertySig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IPropertySymbol propertySymbol && Equals(propertySymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is PropertyInfo propertyInfo && Equals(propertyInfo);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is PropertySig propertySig && Equals(propertySig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PropertySig propertySig) return Equals(propertySig);
        if (obj is IPropertySymbol propertySymbol) return Equals(propertySymbol);
        if (obj is PropertyInfo propertyInfo) return Equals(propertyInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Property, Name, PropertyType);
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .Append(this.Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(this.Keywords)
            .Append(this.PropertyType)
            .Append(' ')
            .If(!IsIndexer, cb => cb.Append(this.Name), cb => cb.Append("this[").DelimitAppend(", ", Parameters).Append(']'))
            .Append(" {")
            .If(HasGetter, b =>
            {
                if (GetMethod!.Visibility != this.Visibility)
                    b.Append(GetMethod.Visibility, "lc").Append(' ');
                b.Append(" get;");
            })
            .If(HasSetter, b =>
            {
                if (SetMethod!.Visibility != this.Visibility)
                    b.Append(SetMethod.Visibility, "lc").Append(' ');
                b.AppendIf(SetMethod!.Keywords.HasFlag(Keywords.Init), " init;", " set;");
            })
            .Append(" }")
            .ToStringAndDispose();
    }
}
