namespace Jay.SourceGen.Reflection;

public abstract class Signature : IEquatable<Signature>
{
    public static implicit operator Signature(FieldInfo fieldInfo) => new FieldSignature(fieldInfo);
    public static implicit operator Signature(PropertyInfo propertyInfo) => new PropertySignature(propertyInfo);
    public static implicit operator Signature(EventInfo eventInfo) => new EventSignature(eventInfo);
    public static implicit operator Signature(ConstructorInfo ctorInfo) => new MethodSignature(ctorInfo);
    public static implicit operator Signature(MethodInfo methodInfo) => new MethodSignature(methodInfo);
    public static implicit operator Signature(ParameterInfo parameterInfo) => new ParameterSignature(parameterInfo);
    public static implicit operator Signature(AttributeData attributeData) => new AttributeSignature(attributeData);
    public static implicit operator Signature(CustomAttributeData customAttrData) => new AttributeSignature(customAttrData);

    public static bool operator ==(Signature? left, Signature? right) => FastEqual(left, right);
    public static bool operator !=(Signature? left, Signature? right) => !FastEqual(left, right);
    
    public static bool TryCreate([AllowNull, NotNullWhen(true)] object? obj, [NotNullWhen(true)] out Signature? signature)
    {
        switch (obj)
        {
            case IFieldSymbol fieldSymbol:
                signature = new FieldSignature(fieldSymbol);
                return true;
            case FieldInfo fieldInfo:
                signature = new FieldSignature(fieldInfo);
                return true;
            case IPropertySymbol propertySymbol:
                signature = new PropertySignature(propertySymbol);
                return true;
            case PropertyInfo propertyInfo:
                signature = new PropertySignature(propertyInfo);
                return true;
            case IEventSymbol eventSymbol:
                signature = new EventSignature(eventSymbol);
                return true;
            case EventInfo eventInfo:
                signature = new EventSignature(eventInfo);
                return true;
            case IMethodSymbol methodSymbol:
                signature = new MethodSignature(methodSymbol);
                return true;
            case MethodBase methodBase:
                signature = new MethodSignature(methodBase);
                return true;
            case Type type:
                signature = new TypeSignature(type);
                return true;
            case IParameterSymbol parameterSymbol:
                signature = new ParameterSignature(parameterSymbol);
                return true;
            case ParameterInfo parameterInfo:
                signature = new ParameterSignature(parameterInfo);
                return true;
            case AttributeData attributeData:
                signature = new AttributeSignature(attributeData);
                return true;
            case CustomAttributeData customAttrData:
                signature = new AttributeSignature(customAttrData);
                return true;
            default:
                signature = default;
                return false;
        }
    }
    
    public string Name { get; }
    public Visibility Visibility { get; }
    public Keywords Keywords { get; }

    protected Signature(string name, Visibility visibility, Keywords keywords)
    {
        this.Name = name;
        this.Visibility = visibility;
        this.Keywords = keywords;
    }

    public virtual bool Equals(Signature? signature)
    {
        return signature is not null 
            && TypeEqual(this, signature) 
            && TextEqual(Name, signature.Name) 
            && FastEqual(Visibility, signature.Visibility) 
            && FastEqual(Keywords, signature.Keywords);
    }
    
    public override bool Equals(object? obj)
    {
        return TryCreate(obj, out var signature) && Equals(signature);
    }

    /// <summary>
    /// <b>WARNING</b>: <see cref="Signature"/> and derivatives are mutable!
    /// </summary>
    public override int GetHashCode()
    {
        return Hasher.Combine(GetType(), Name, Visibility, Keywords);
    }
}