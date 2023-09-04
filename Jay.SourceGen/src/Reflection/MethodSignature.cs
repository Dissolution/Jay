namespace Jay.SourceGen.Reflection;

public sealed class MethodSignature : MemberSignature,
    IEquatable<MethodSignature>, IEquatable<IMethodSymbol>, IEquatable<MethodBase>
{
    [return: NotNullIfNotNull(nameof(methodBase))]
    public static implicit operator MethodSignature?(MethodBase? methodBase) => Create(methodBase);


    public static bool operator ==(MethodSignature? left, MethodSignature? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSignature? left, MethodSignature? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(MethodSignature? left, IMethodSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSignature? left, IMethodSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(MethodSignature? left, MethodBase? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(MethodSignature? left, MethodBase? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    public static MethodSignature? Create(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol is null) return null;
        return new MethodSignature(methodSymbol);
    }
    public static MethodSignature? Create(MethodBase? methodBase)
    {
        if (methodBase is null) return null;
        return new MethodSignature(methodBase);
    }


    public TypeSignature? ReturnType { get; set; } = null;

    public IReadOnlyList<ParameterSignature> Parameters { get; set; } = Array.Empty<ParameterSignature>();

    public MethodSignature()
        : base(SigType.Method)
    {

    }

    public MethodSignature(IMethodSymbol methodSymbol)
        : base(SigType.Method, methodSymbol)
    {
        ReturnType = new TypeSignature(methodSymbol.ReturnType);
        Parameters = methodSymbol.Parameters.Select(static p => new ParameterSignature(p)).ToList();
    }

    public MethodSignature(MethodBase methodBase)
        : base(SigType.Method, methodBase)
    {
        ReturnType = TypeSignature.Create(methodBase.GetReturnType());
        Parameters = methodBase.GetParameters().Select(static p => new ParameterSignature(p)).ToList();
    }

    public bool Equals(MethodSignature? methodSig)
    {
        return methodSig is not null &&
            Name == methodSig.Name &&
            ReturnType == methodSig.ReturnType &&
            Parameters.SequenceEqual(methodSig.Parameters);
    }

    public bool Equals(IMethodSymbol? methodSymbol)
    {
        return methodSymbol is not null &&
            Name == methodSymbol.Name &&
           ReturnType == methodSymbol.ReturnType &&
            Parameters.SequenceEqual(methodSymbol.Parameters.Select(static p => new ParameterSignature(p)));
    }

    public bool Equals(MethodBase? methodBase)
    {
        return methodBase is not null &&
            Name == methodBase.Name &&
           ReturnType == methodBase.GetReturnType() &&
            Parameters.SequenceEqual(methodBase.GetParameters().Select(static p => new ParameterSignature(p)));
    }

    public override bool Equals(Signature? signature)
    {
        return signature is MethodSignature methodSig && Equals(methodSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IMethodSymbol methodSymbol && Equals(methodSymbol);
    }

    public override bool Equals(MemberInfo? memberInfo)
    {
        return memberInfo is MethodBase methodBase && Equals(methodBase);
    }

    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is MethodSignature methodSig && Equals(methodSig);
    }

    public override bool Equals(object? obj)
    {
        if (obj is MethodSignature methodSig) return Equals(methodSig);
        if (obj is IMethodSymbol methodSymbol) return Equals(methodSymbol);
        if (obj is MethodBase methodBase) return Equals(methodBase);
        return false;
    }

    public override int GetHashCode()
    {
        Hasher hasher = new();
        hasher.Add(SigType.Method);
        hasher.Add(Name);
        hasher.Add(ReturnType);
        hasher.AddAll<ParameterSignature>(Parameters);
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        return CodeBuilder.New
          .Append(Visibility, "lc")
          .AppendIf(this.Instic == Instic.Static, " static ", " ")
          .AppendKeywords(Keywords)
          .Append(ReturnType).Append(' ')
          .Append(Name).Append('(')
          .DelimitAppend(", ", Parameters)
          .Append(')')
          .ToStringAndDispose();
    }
}