namespace Jay.SourceGen.Reflection;

public abstract class MemberSig : Sig,
    IEquatable<MemberSig>, IEquatable<ISymbol>, IEquatable<MemberInfo>
{
    [return: NotNullIfNotNull(nameof(memberInfo))]
    public static implicit operator MemberSig?(MemberInfo? memberInfo) => MemberSig.Create(memberInfo);

    public static bool operator ==(MemberSig? left, MemberSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(MemberSig? left, MemberSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }
    public static bool operator ==(MemberSig? left, ISymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MemberSig? left, ISymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(MemberSig? left, MemberInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MemberSig? left, MemberInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public static MemberSig? Create(ISymbol? symbol)
    {
        return symbol switch
        {
            IFieldSymbol fieldSymbol => new FieldSig(fieldSymbol),
            IPropertySymbol propertySymbol => new PropertySig(propertySymbol),
            IEventSymbol eventSymbol => new EventSig(eventSymbol),
            IMethodSymbol methodSymbol => new MethodSig(methodSymbol),
            ITypeSymbol typeSymbol => new TypeSig(typeSymbol),
            _ => throw new ArgumentException(),
        };
    }

    public static MemberSig? Create(MemberInfo? member)
    {
        return member switch
        {
            null => null,
            FieldInfo fieldInfo => new FieldSig(fieldInfo),
            PropertyInfo propertyInfo => new PropertySig(propertyInfo),
            EventInfo eventInfo => new EventSig(eventInfo),
            ConstructorInfo ctorInfo => new MethodSig(ctorInfo),
            MethodInfo methodInfo => new MethodSig(methodInfo),
            Type type => new TypeSig(type),
            _ => throw new ArgumentException(),
        };
    }



    public TypeSig? ParentType { get; set; } = null;
    public IReadOnlyList<AttributeSig> Attributes {get;set;} = Array.Empty<AttributeSig>();

    protected MemberSig(SigType sigType) : base(sigType)
    {

    }

    protected MemberSig(SigType sigType, ISymbol symbol) : base(sigType)
    {
        this.Name = symbol.Name;
        this.Visibility = symbol.GetVisibility();
        this.Instic = symbol.GetInstic();
        this.Keywords = symbol.GetKeywords();

        this.ParentType = TypeSig.Create(symbol.ContainingType);
        this.Attributes = symbol.GetAttributes().Select(static a => AttributeSig.Create(a)).ToList();
    }

    protected MemberSig(SigType sigType, MemberInfo member) : base(sigType)
    {
        this.Name = member.Name;
        this.Visibility = member.GetVisibility();
        this.Instic = member.GetInstic();
        this.Keywords = member.GetKeywords();

        this.ParentType = TypeSig.Create(member.ReflectedType ?? member.DeclaringType);
        this.Attributes = member.GetCustomAttributesData().Select(static a => AttributeSig.Create(a)).ToList();
    }


    public bool HasAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return this.Attributes.Any(attr => attr.Name == typeof(TAttribute).Name);
    }


    public virtual bool Equals(MemberSig? memberSig)
    {
        return memberSig is not null &&
            this.SigType == memberSig.SigType &&
            string.Equals(this.Name, memberSig.Name);
    }

    public virtual bool Equals(MemberInfo? memberInfo)
    {
        return Equals(MemberSig.Create(memberInfo));
    }

    public override bool Equals(Sig? signature)
    {
        return signature is MemberSig memberSig && Equals(memberSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return Equals(MemberSig.Create(symbol));
    }

    public override bool Equals(object? obj)
    {
        if (obj is MemberSig memberSig) return Equals(memberSig);
        if (obj is ISymbol symbol) return Equals(symbol);
        if (obj is MemberInfo member) return Equals(member);
        return false;
    }

    public override int GetHashCode()
    {
        // same as base
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .If(Attributes.Count > 0, b => b.Append('[').DelimitAppend(", ", Attributes).AppendLine(']'))
            .Append(this.Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(this.Keywords)
            .Append(this.SigType, "lc").Append(' ').Append(this.Name)
            .ToStringAndDispose();
    }
}