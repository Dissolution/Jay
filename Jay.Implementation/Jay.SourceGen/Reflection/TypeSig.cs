namespace Jay.SourceGen.Reflection;

public sealed class TypeSig : MemberSig,
    IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
{
    [return: NotNullIfNotNull(nameof(type))]
    public static implicit operator TypeSig?(Type? type) => TypeSig.Create(type);

    public static bool operator ==(TypeSig? left, TypeSig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, TypeSig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(TypeSig? left, ITypeSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, ITypeSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(TypeSig? left, Type? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(TypeSig? left, Type? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(typeSymbol))]
    public static TypeSig? Create(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return null;
        return new TypeSig(typeSymbol);
    }
    [return: NotNullIfNotNull(nameof(type))]
    public static TypeSig? Create(Type? type)
    {
        if (type is null) return null;
        return new TypeSig(type);
    }

    public string? Namespace { get; set; } = null;
    public string? FullName { get; set; } = null;
    public ObjType ObjType {get;set;} = default;

    public TypeSig(ITypeSymbol typeSymbol)
        : base(SigType.Type, typeSymbol)
    {
        this.Namespace = typeSymbol.GetNamespace();
        this.FullName = typeSymbol.GetFullName();
        this.Name = typeSymbol.ToString();
        if (typeSymbol.IsValueType)
        {
            this.ObjType = ObjType.Struct;
        }
        else
        {
            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Delegate:
                {
                    this.ObjType = ObjType.Delegate;
                    break;
                }
                case TypeKind.Interface:
                {
                    this.ObjType = ObjType.Interface;
                    break;
                }
                case TypeKind.Class:
                {
                    this.ObjType = ObjType.Class;
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public TypeSig(Type type)
       : base(SigType.Type, type)
    {
        this.Namespace = type.Namespace;
        this.FullName = type.FullName;
        this.Name = type.ToString();
        if (type.IsValueType)
        {
            this.ObjType = ObjType.Struct;
        }
        else if (typeof(Delegate).IsAssignableFrom(type))
        {
            this.ObjType = ObjType.Delegate;
        }
        else if (type.IsInterface)
        {
            this.ObjType = ObjType.Interface;
        }
        else if (type.IsClass)
        {
            this.ObjType = ObjType.Class;
        }
        else
            throw new NotImplementedException();
    }

    public TypeSig()
       : base(SigType.Type)
    {

    }

    public bool Equals(TypeSig? typeSig)
    {
        return typeSig is not null &&
            string.Equals(this.FullName, typeSig.FullName);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is TypeSig typeSig && Equals(typeSig);
    }

    public bool Equals(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is not null &&
            string.Equals(this.FullName, typeSymbol.GetFullName());
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is ITypeSymbol typeSymbol && Equals(typeSymbol);
    }

    public bool Equals(Type? type)
    {
        return type is not null &&
                   string.Equals(this.FullName, type.FullName);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is Type type && Equals(type);
    }

    public override bool Equals(Sig? signature)
    {
        return signature is TypeSig typeSig && Equals(typeSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TypeSig typeSig) return Equals(typeSig);
        if (obj is ITypeSymbol typeSymbol) return Equals(typeSymbol);
        if (obj is Type type) return Equals(type);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Type, FullName);
    }

    public override string ToString()
    {
        return Name;
    }

}