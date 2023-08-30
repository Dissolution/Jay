namespace Jay.SourceGen.Reflection;

public sealed class MethodSig : MemberSig,
    IEquatable<MethodSig>, IEquatable<IMethodSymbol>, IEquatable<MethodBase>
{
    [return: NotNullIfNotNull(nameof(methodBase))]
    public static implicit operator MethodSig?(MethodBase? methodBase) => MethodSig.Create(methodBase);


    public static bool operator ==(MethodSig? left, MethodSig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSig? left, MethodSig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(MethodSig? left, IMethodSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSig? left, IMethodSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(MethodSig? left, MethodBase? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSig? left, MethodBase? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public static MethodSig? Create(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol is null) return null;
        return new MethodSig(methodSymbol);
    }
    public static MethodSig? Create(MethodBase? methodBase)
    {
        if (methodBase is null) return null;
        return new MethodSig(methodBase);
    }


    public TypeSig? ReturnType { get; set; } = null;

    public IReadOnlyList<ParameterSig> Parameters { get; set; } = Array.Empty<ParameterSig>();

    public MethodSig()
        : base(SigType.Method)
    {

    }

    public MethodSig(IMethodSymbol methodSymbol)
        : base(SigType.Method, methodSymbol)
    {
        this.ReturnType = new TypeSig(methodSymbol.ReturnType);
        this.Parameters = methodSymbol.Parameters.Select(static p => new ParameterSig(p)).ToList();
    }

    public MethodSig(MethodBase methodBase)
        : base(SigType.Method, methodBase)
    {
        this.ReturnType = TypeSig.Create(methodBase.GetReturnType());
        this.Parameters = methodBase.GetParameters().Select(static p => new ParameterSig(p)).ToList();
    }

    public bool Equals(MethodSig? methodSig)
    {
        return methodSig is not null &&
            this.Name == methodSig.Name &&
            this.ReturnType == methodSig.ReturnType &&
            this.Parameters.SequenceEqual(methodSig.Parameters);
    }

    public bool Equals(IMethodSymbol? methodSymbol)
    {
        return methodSymbol is not null &&
            this.Name == methodSymbol.Name &&
           this.ReturnType == methodSymbol.ReturnType &&
            this.Parameters.SequenceEqual(methodSymbol.Parameters.Select(static p => new ParameterSig(p)));
    }

    public bool Equals(MethodBase? methodBase)
    {
        return methodBase is not null &&
            this.Name == methodBase.Name &&
           this.ReturnType == methodBase.GetReturnType() &&
            this.Parameters.SequenceEqual(methodBase.GetParameters().Select(static p => new ParameterSig(p)));
    }

    public override bool Equals(Sig? signature)
    {
        return signature is MethodSig methodSig && Equals(methodSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IMethodSymbol methodSymbol && Equals(methodSymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is MethodBase methodBase && Equals(methodBase);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is MethodSig methodSig && Equals(methodSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is MethodSig methodSig) return Equals(methodSig);
        if (obj is IMethodSymbol methodSymbol) return Equals(methodSymbol);
        if (obj is MethodBase methodBase) return Equals(methodBase);
        return false;
    }

    public override int GetHashCode()
    {
        Hasher hasher = new();
        hasher.Add(SigType.Method);
        hasher.Add(this.Name);
        hasher.Add(this.ReturnType);
        hasher.AddAll<ParameterSig>(this.Parameters);
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        return CodeBuilder.New
          .Append(this.Visibility, "lc")
          .AppendIf(this.Instic == Instic.Static, " static ", " ")
          .AppendKeywords(this.Keywords)
          .Append(this.ReturnType).Append(' ')
          .Append(this.Name).Append('(')
          .DelimitAppend(", ", this.Parameters)
          .Append(')')
          .ToStringAndDispose();
    }
}