namespace Jay.SourceGen.Reflection;

public sealed class FieldSignature : MemberSignature,
    IEquatable<FieldSignature>, IEquatable<IFieldSymbol>, IEquatable<FieldInfo>
{
    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static implicit operator FieldSignature?(FieldInfo? fieldInfo) => Create(fieldInfo);
    
    public static bool operator ==(FieldSignature? left, FieldSignature? right) => FastEqual(left, right);
    public static bool operator !=(FieldSignature? left, FieldSignature? right) => !FastEqual(left, right);
    public static bool operator ==(FieldSignature? left, IFieldSymbol? right) => FastEquality(left, right);
    public static bool operator !=(FieldSignature? left, IFieldSymbol? right) => !FastEquality(left, right);
    public static bool operator ==(FieldSignature? left, FieldInfo? right) => FastEquality(left, right);
    public static bool operator !=(FieldSignature? left, FieldInfo? right) => !FastEquality(left, right);
    
    
    [return: NotNullIfNotNull(nameof(fieldSymbol))]
    public static FieldSignature? Create(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol is null) return null;
        return new FieldSignature(fieldSymbol);
    }
    [return: NotNullIfNotNull(nameof(fieldInfo))]
    public static FieldSignature? Create(FieldInfo? fieldInfo)
    {
        if (fieldInfo is null) return null;
        return new FieldSignature(fieldInfo);
    }

    public TypeSignature FieldType { get; init; }
    
    internal FieldSignature(IFieldSymbol fieldSymbol)
        : base(fieldSymbol)
    {
        this.FieldType = TypeSignature.Create(fieldSymbol.Type);
    }

    internal FieldSignature(FieldInfo fieldInfo)
        : base(fieldInfo)
    {
        this.FieldType = TypeSignature.Create(fieldInfo.FieldType);
    }

    public bool Equals(FieldSignature? fieldSig)
    {
        return base.Equals(fieldSig)
            && FastEqual(FieldType, fieldSig.FieldType);
    }
    
    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is FieldSignature fieldSig && Equals(fieldSig);
    }
    
    public override bool Equals(Signature? signature)
    {
        return signature is FieldSignature fieldSig && Equals(fieldSig);
    }

    public bool Equals(IFieldSymbol? fieldSymbol)
    {
        return Equals(Create(fieldSymbol));
    }
    
    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol);
    }

    public bool Equals(FieldInfo? fieldInfo)
    {
        return Equals(Create(fieldInfo));
    }
    
    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is FieldInfo fieldInfo && Equals(fieldInfo);
    }


    public override bool Equals(object? obj)
    {
        return obj switch
        {
            FieldSignature fieldSig => Equals(fieldSig),
            IFieldSymbol fieldSymbol => Equals(fieldSymbol),
            FieldInfo fieldInfo => Equals(fieldInfo),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(GetType(), Name, Visibility, Keywords, FieldType);
    }

    public override void DeclareTo(CodeBuilder code)
    {
        code.Code(Attributes)
            .Code(Visibility)
            .Write(' ')
            .Code(Keywords)
            .Write(' ')
            .Code(FieldType)
            .Write(' ')
            .Write(Name)
            .Write(';');
    }
}
