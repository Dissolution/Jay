namespace Jay.SourceGen.Reflection;

public sealed class MethodSignature :
    MemberSignature,
    IEquatable<MethodSignature>,
    IEquatable<IMethodSymbol>,
    IEquatable<MethodBase>
{
    [return: NotNullIfNotNull(nameof(methodBase))]
    public static implicit operator MethodSignature?(MethodBase? methodBase) => Create(methodBase);


    public static bool operator ==(MethodSignature? left, MethodSignature? right) => FastEqual(left, right);
    public static bool operator !=(MethodSignature? left, MethodSignature? right) => !FastEqual(left, right);
    public static bool operator ==(MethodSignature? left, IMethodSymbol? right) => FastEquality(left, right);
    public static bool operator !=(MethodSignature? left, IMethodSymbol? right) => !FastEquality(left, right);
    public static bool operator ==(MethodSignature? left, MethodBase? right) => FastEquality(left, right);
    public static bool operator !=(MethodSignature? left, MethodBase? right) => !FastEquality(left, right);

    public static MethodSignature? Create(IMethodSymbol? methodSymbol)
    {
        if (methodSymbol is null)
            return null;

        return new MethodSignature(methodSymbol);
    }

    public static MethodSignature? Create(MethodBase? methodBase)
    {
        if (methodBase is null)
            return null;

        return new MethodSignature(methodBase);
    }


    public TypeSignature? ReturnType { get; }

    public IReadOnlyList<ParameterSignature> Parameters { get; }

    public MethodSignature(IMethodSymbol methodSymbol)
        : base(methodSymbol)
    {
        this.ReturnType = TypeSignature.Create(methodSymbol.ReturnType);
        this.Parameters = methodSymbol.Parameters
            .Select(static p => new ParameterSignature(p))
            .ToList();
    }

    public MethodSignature(MethodBase methodBase)
        : base(methodBase)
    {
        this.ReturnType = TypeSignature.Create(methodBase.ReturnType());
        this.Parameters = methodBase.GetParameters()
            .Select(static p => new ParameterSignature(p))
            .ToList();
    }

    public bool Equals(MethodSignature? methodSig)
    {
        return base.Equals(methodSig)
            && FastEqual(ReturnType, methodSig.ReturnType)
            && SeqEqual(Parameters, methodSig.Parameters);
    }
    
    public override bool Equals(MemberSignature? memberSig)
    {
        return memberSig is MethodSignature methodSig && Equals(methodSig);
    }
    
    public override bool Equals(Signature? signature)
    {
        return signature is MethodSignature methodSig && Equals(methodSig);
    }

    public bool Equals(IMethodSymbol? methodSymbol) => Equals(Create(methodSymbol));

    public bool Equals(MethodBase? methodBase) => Equals(Create(methodBase));

    public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));

    public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));
    
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            MethodSignature methodSig => Equals(methodSig),
            IMethodSymbol methodSymbol => Equals(methodSymbol),
            MethodBase methodBase => Equals(methodBase),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        Hasher hasher = new();
        hasher.Add(base.GetHashCode());
        hasher.Add(ReturnType);
        hasher.AddAll<ParameterSignature>(Parameters);
        return hasher.ToHashCode();
    }

    public override void DeclareTo(CodeBuilder code)
    {
        code.Code(Attributes)
            .Code(Visibility)
            .Write(' ')
            .Code(Keywords)
            .Write(' ')
            .Code(ReturnType)
            .Write(' ')
            .Write(Name)
            .Write('(')
            .Delimit(", ", Parameters, static (cb, p) => cb.Code(p))
            .Write(");");
    }
}