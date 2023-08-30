namespace Jay.SourceGen.Reflection;

public sealed class ParameterSig : Sig,
    IEquatable<ParameterSig>, IEquatable<IParameterSymbol>, IEquatable<ParameterInfo>
{
    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static implicit operator ParameterSig?(ParameterInfo? parameterInfo) => ParameterSig.Create(parameterInfo);

    public static bool operator ==(ParameterSig? left, ParameterSig? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSig? left, ParameterSig? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(ParameterSig? left, IParameterSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSig? left, IParameterSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(ParameterSig? left, ParameterInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSig? left, ParameterInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(parameterSymbol))]
    public static ParameterSig? Create(IParameterSymbol? parameterSymbol)
    {
        if (parameterSymbol is null) return null;
        return new ParameterSig(parameterSymbol);
    }
    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static ParameterSig? Create(ParameterInfo? parameterInfo)
    {
        if (parameterInfo is null) return null;
        return new ParameterSig(parameterInfo);
    }

    public TypeSig? ParameterType { get; set; } = null;
    public bool HasDefaultValue { get; set; } = false;
    public object? DefaultValue { get; set; } = null;
    public bool IsParams {get;set;} = false;

    public ParameterSig()
        : base(SigType.Parameter)
    {

    }

    public ParameterSig(IParameterSymbol parameterSymbol)
        : base(SigType.Parameter)
    {
        this.Name = parameterSymbol.Name;
        this.Visibility = Enums.Visibility.Public;
        this.Instic = Instic.Instance;
        this.Keywords = default;
        this.ParameterType = new TypeSig(parameterSymbol.Type);
        this.HasDefaultValue = parameterSymbol.HasExplicitDefaultValue;
        if (HasDefaultValue)
        {
            this.DefaultValue = parameterSymbol.ExplicitDefaultValue;
        }
        this.IsParams = parameterSymbol.IsParams;
    }

    public ParameterSig(ParameterInfo parameterInfo)
        : base(SigType.Parameter)
    {
        this.Name = parameterInfo.Name;
        this.Visibility = Enums.Visibility.Public;
        this.Instic = Instic.Instance;
        this.Keywords = default;
        this.ParameterType = new TypeSig(parameterInfo.ParameterType);
        this.HasDefaultValue = parameterInfo.HasDefaultValue;
        if (HasDefaultValue)
        {
            this.DefaultValue = parameterInfo.DefaultValue;
        }
        this.IsParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
    }

    public bool Equals(ParameterSig? parameterSig)
    {
        return parameterSig is not null &&
            this.Name == parameterSig.Name &&
            this.ParameterType == parameterSig.ParameterType;
    }

    public bool Equals(IParameterSymbol? parameterSymbol)
    {
        return parameterSymbol is not null &&
            this.Name == parameterSymbol.Name &&
            this.ParameterType == parameterSymbol.Type;
    }

    public bool Equals(ParameterInfo? parameterInfo)
    {
        return parameterInfo is not null &&
            this.Name == parameterInfo.Name &&
            this.ParameterType == parameterInfo.ParameterType;
    }

    public override bool Equals(Sig? signature)
    {
        return signature is ParameterSig parameterSig && Equals(parameterSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IParameterSymbol parameterSymbol && Equals(parameterSymbol);
    }

    public override bool Equals(object? obj)
    {
        if (obj is ParameterSig parameterSig) return Equals(parameterSig);
        if (obj is IParameterSymbol parameterSymbol) return Equals(parameterSymbol);
        if (obj is ParameterInfo parameterInfo) return Equals(parameterInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(SigType.Parameter, Name, ParameterType);
    }

    public override string ToString()
    {
        return $"{(IsParams ? "params " : "")}{ParameterType} {Name}{(HasDefaultValue ? $" = {DefaultValue}" : "")}";
    }
}