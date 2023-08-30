namespace Jay.SourceGen.Reflection;

public abstract class Sig : IEquatable<Sig>, IEquatable<ISymbol>
{
    public static bool operator ==(Sig? left, Sig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(Sig? left, Sig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(Sig? left, ISymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(Sig? left, ISymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public static bool TryCreate([AllowNull, NotNullWhen(true)] object? reflection, [NotNullWhen(true)] out Sig? signature)
    {
        switch (reflection)
        {
            case IFieldSymbol fieldSymbol:
                signature = new FieldSig(fieldSymbol);
                return true;
            case FieldInfo fieldInfo:
                signature = new FieldSig(fieldInfo);
                return true;
            case IPropertySymbol propertySymbol:
                signature = new PropertySig(propertySymbol);
                return true;
            case PropertyInfo propertyInfo:
                signature = new PropertySig(propertyInfo);
                return true;
            case IEventSymbol eventSymbol:
                signature = new EventSig(eventSymbol);
                return true;
            case EventInfo eventInfo:
                signature = new EventSig(eventInfo);
                return true;
            case IMethodSymbol methodSymbol:
                signature = new MethodSig(methodSymbol);
                return true;
            case ConstructorInfo ctorInfo:
                signature = new MethodSig(ctorInfo);
                return true;
            case MethodInfo methodInfo:
                signature = new MethodSig(methodInfo);
                return true;
            case IParameterSymbol parameterSymbol:
                signature = new ParameterSig(parameterSymbol);
                return true;
            case ParameterInfo parameterInfo:
                signature = new ParameterSig(parameterInfo);
                return true;
            case AttributeData attributeData:
                signature = new AttributeSig(attributeData);
                return true;
            case CustomAttributeData customAttrData:
                signature = new AttributeSig(customAttrData);
                return true;
            default:
            {
                signature = default;
                return false;
            }
        }
    }


    public SigType SigType { get; }
    public string? Name { get; set; } = null;

    public Visibility Visibility { get; set; } = default;
    public Instic Instic { get; set; } = default;
    public Keywords Keywords { get; set; } = default;

    protected Sig(SigType sigType)
    {
        this.SigType = sigType;
    }

    public virtual bool Equals(Sig? signature)
    {
        return signature is not null &&
            this.SigType == signature.SigType &&
            string.Equals(this.Name, signature.Name) &&
            this.Visibility == signature.Visibility &&
            this.Instic == signature.Instic &&
            this.Keywords == signature.Keywords;
    }

    public virtual bool Equals(ISymbol? symbol)
    {
        return TryCreate(symbol, out var signature) && Equals(signature);
    }

    public override bool Equals(object? obj)
    {
        return TryCreate(obj, out var signature) && Equals(signature);
    }

    /// <summary>
    /// <b>WARNING</b>: <see cref="Sig"/> and derivatives are mutable!
    /// </summary>
    public override int GetHashCode()
    {
        return Hasher.Create(SigType, Name, Visibility, Instic, Keywords);
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .Append(this.Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(this.Keywords)
            .Append(this.SigType, "lc").Append(' ').Append(this.Name)
            .ToStringAndDispose();
    }
}
