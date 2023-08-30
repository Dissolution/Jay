namespace Jay.SourceGen.Reflection;

public sealed class FieldSig : MemberSig,
    IEquatable<FieldSig>, IEquatable<IFieldSymbol>, IEquatable<FieldInfo>
{
    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static implicit operator FieldSig?(FieldInfo? fieldInfo) => FieldSig.Create(fieldInfo);


    public static bool operator ==(FieldSig? left, FieldSig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSig? left, FieldSig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(FieldSig? left, IFieldSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSig? left, IFieldSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(FieldSig? left, FieldInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(FieldSig? left, FieldInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(fieldSymbol))]
    public static FieldSig? Create(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol is null) return null;
        return new FieldSig(fieldSymbol);
    }
    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static FieldSig? Create(FieldInfo? fieldInfo)
    {
        if (fieldInfo is null) return null;
        return new FieldSig(fieldInfo);
    }

    public TypeSig? FieldType { get; set; } = null;

    public FieldSig()
        : base(SigType.Field)
    {

    }

    public FieldSig(IFieldSymbol fieldSymbol)
        : base(SigType.Field, fieldSymbol)
    {
        this.FieldType = new TypeSig(fieldSymbol.Type);
    }

    public FieldSig(FieldInfo fieldInfo)
        : base(SigType.Field, fieldInfo)
    {
        this.FieldType = new TypeSig(fieldInfo.FieldType);
    }

    public bool Equals(FieldSig? fieldSig)
    {
        return fieldSig is not null &&
            this.Name == fieldSig.Name &&
            this.FieldType == fieldSig.FieldType;
    }

    public bool Equals(IFieldSymbol? fieldSymbol)
    {
        return fieldSymbol is not null &&
            this.Name == fieldSymbol.Name &&
            this.FieldType == fieldSymbol.Type;
    }

    public bool Equals(FieldInfo? fieldInfo)
    {
        return fieldInfo is not null &&
            this.Name == fieldInfo.Name &&
            this.FieldType == fieldInfo.FieldType;
    }

    public override bool Equals(Sig? signature)
    {
        return signature is FieldSig fieldSig && Equals(fieldSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is FieldInfo fieldInfo && Equals(fieldInfo);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is FieldSig fieldSig && Equals(fieldSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is FieldSig fieldSig) return Equals(fieldSig);
        if (obj is IFieldSymbol fieldSymbol) return Equals(fieldSymbol);
        if (obj is FieldInfo fieldInfo) return Equals(fieldInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Field, Name, FieldType);
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .Append(this.Visibility, "lc")
            .AppendIf(this.Instic == Instic.Static, " static ", " ")
            .AppendKeywords(this.Keywords)
            .Append(this.FieldType).Append(' ').Append(this.Name)
            .ToStringAndDispose();
    }
}
