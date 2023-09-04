namespace Jay.SourceGen.Reflection;

public abstract class MemberSignature : Signature,
    IEquatable<MemberSignature>, IEquatable<ISymbol>, IEquatable<MemberInfo>
{
    [return: NotNullIfNotNull(nameof(memberInfo))]
    public static implicit operator MemberSignature?(MemberInfo? memberInfo) => MemberSignature.Create(memberInfo);

    public static bool operator ==(MemberSignature? left, MemberSignature? right) => FastEqual(left, right);
    public static bool operator !=(MemberSignature? left, MemberSignature? right) => !FastEqual(left, right);
    public static bool operator ==(MemberSignature? left, ISymbol? right) => FastEquality(left, right);
    public static bool operator !=(MemberSignature? left, ISymbol? right) => !FastEquality(left, right);
    public static bool operator ==(MemberSignature? left, MemberInfo? right) => FastEquality(left, right);
    public static bool operator !=(MemberSignature? left, MemberInfo? right) => !FastEquality(left, right);


    [return: NotNullIfNotNull(nameof(symbol))]
    public static MemberSignature? Create(ISymbol? symbol)
    {
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
                return new FieldSignature(fieldSymbol);
            case IPropertySymbol propertySymbol:
                return new PropertySignature(propertySymbol);
            case IEventSymbol eventSymbol:
                return new EventSignature(eventSymbol);
            case IMethodSymbol methodSymbol:
                return new MethodSignature(methodSymbol);
            case ITypeSymbol typeSymbol:
                return new TypeSignature(typeSymbol);
            default:
                return default;
        }
    }

    [return: NotNullIfNotNull(nameof(member))]
    public static MemberSignature? Create(MemberInfo? member)
    {
        switch (member)
        {
            case FieldInfo fieldInfo:
                return new FieldSignature(fieldInfo);
            case PropertyInfo propertyInfo:
                return new PropertySignature(propertyInfo);
            case EventInfo eventInfo:
                return new EventSignature(eventInfo);
            case MethodBase methodBase:
                return new MethodSignature(methodBase);
            case Type type:
                return new TypeSignature(type);
            default:
                return default;
        }
    }

    public static bool TryCreate([AllowNull, NotNullWhen(true)] object? obj, [NotNullWhen(true)] out MemberSignature? memberSignature)
    {
        switch (obj)
        {
            case IFieldSymbol fieldSymbol:
                memberSignature = new FieldSignature(fieldSymbol);
                return true;
            case FieldInfo fieldInfo:
                memberSignature = new FieldSignature(fieldInfo);
                return true;
            case IPropertySymbol propertySymbol:
                memberSignature = new PropertySignature(propertySymbol);
                return true;
            case PropertyInfo propertyInfo:
                memberSignature = new PropertySignature(propertyInfo);
                return true;
            case IEventSymbol eventSymbol:
                memberSignature = new EventSignature(eventSymbol);
                return true;
            case EventInfo eventInfo:
                memberSignature = new EventSignature(eventInfo);
                return true;
            case IMethodSymbol methodSymbol:
                memberSignature = new MethodSignature(methodSymbol);
                return true;
            case MethodBase methodBase:
                memberSignature = new MethodSignature(methodBase);
                return true;
            case Type type:
                memberSignature = new TypeSignature(type);
                return true;
            default:
                memberSignature = default;
                return false;
        }
    }


    public TypeSignature? ParentType { get; init; }
    public SignatureAttributes Attributes { get; init; }

    protected MemberSignature(ISymbol symbol) : base(symbol.Name, symbol.GetVisibility(), symbol.GetKeywords())
    {
        this.ParentType = TypeSignature.Create(symbol.ContainingType);
        this.Attributes = new(symbol);
    }

    protected MemberSignature(MemberInfo member) : base(member.Name, member.GetVisibility(), member.GetKeywords())
    {
        this.ParentType = TypeSignature.Create(member.ReflectedType ?? member.DeclaringType);
        this.Attributes = new(member);
    }


    public override bool Equals(Signature? signature)
    {
        return signature is MemberSignature memberSig && Equals(memberSig);
    }

    public virtual bool Equals(MemberSignature? memberSig)
    {
        return base.Equals(memberSig)
            && FastEqual(ParentType, memberSig.ParentType)
            && FastEqual(Attributes, memberSig.Attributes);
    }

    public virtual bool Equals(MemberInfo? memberInfo)
    {
        return Equals(Create(memberInfo));
    }

    public virtual bool Equals(ISymbol? symbol)
    {
        return Equals(Create(symbol));
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            MemberSignature memberSig => Equals(memberSig),
            ISymbol symbol => Equals(symbol),
            MemberInfo member => Equals(member),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(base.GetHashCode(), ParentType, Attributes);
    }
}