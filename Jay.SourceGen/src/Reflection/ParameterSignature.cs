namespace Jay.SourceGen.Reflection;

public sealed class ParameterSignature : Signature,
    IEquatable<ParameterSignature>, IEquatable<IParameterSymbol>, IEquatable<ParameterInfo>
{
    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static implicit operator ParameterSignature?(ParameterInfo? parameterInfo) => Create(parameterInfo);

    public static bool operator ==(ParameterSignature? left, ParameterSignature? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSignature? left, ParameterSignature? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(ParameterSignature? left, IParameterSymbol? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSignature? left, IParameterSymbol? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }
    public static bool operator ==(ParameterSignature? left, ParameterInfo? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    public static bool operator !=(ParameterSignature? left, ParameterInfo? right)
    {
        if (left is null) return right is not null;
        return !left.Equals(right);
    }

    [return: NotNullIfNotNull(nameof(parameterSymbol))]
    public static ParameterSignature? Create(IParameterSymbol? parameterSymbol)
    {
        if (parameterSymbol is null) return null;
        return new ParameterSignature(parameterSymbol);
    }
    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static ParameterSignature? Create(ParameterInfo? parameterInfo)
    {
        if (parameterInfo is null) return null;
        return new ParameterSignature(parameterInfo);
    }

    public TypeSignature? ParameterType { get; set; } = null;
    public bool HasDefaultValue { get; set; } = false;
    public object? DefaultValue { get; set; } = null;
    public bool IsParams {get;set;} = false;

    public ParameterSignature()
        : base(SigType.Parameter)
    {

    }

    public ParameterSignature(IParameterSymbol parameterSymbol)
        : base(SigType.Parameter)
    {
        Name = parameterSymbol.Name;
        Visibility = Enums.Visibility.Public;
        this.Instic = Instic.Instance;
        Keywords = default;
        ParameterType = new TypeSignature(parameterSymbol.Type);
        HasDefaultValue = parameterSymbol.HasExplicitDefaultValue;
        if (HasDefaultValue)
        {
            DefaultValue = parameterSymbol.ExplicitDefaultValue;
        }
        IsParams = parameterSymbol.IsParams;
    }

    public ParameterSignature(ParameterInfo parameterInfo)
        : base(SigType.Parameter)
    {
        Name = parameterInfo.Name;
        Visibility = Enums.Visibility.Public;
        this.Instic = Instic.Instance;
        Keywords = default;
        ParameterType = new TypeSignature(parameterInfo.ParameterType);
        HasDefaultValue = parameterInfo.HasDefaultValue;
        if (HasDefaultValue)
        {
            DefaultValue = parameterInfo.DefaultValue;
        }
        IsParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
    }

    public bool Equals(ParameterSignature? parameterSig)
    {
        return parameterSig is not null &&
            Name == parameterSig.Name &&
            ParameterType == parameterSig.ParameterType;
    }

    public bool Equals(IParameterSymbol? parameterSymbol)
    {
        return parameterSymbol is not null &&
            Name == parameterSymbol.Name &&
            ParameterType == parameterSymbol.Type;
    }

    public bool Equals(ParameterInfo? parameterInfo)
    {
        return parameterInfo is not null &&
            Name == parameterInfo.Name &&
            ParameterType == parameterInfo.ParameterType;
    }

    public override bool Equals(Signature? signature)
    {
        return signature is ParameterSignature parameterSig && Equals(parameterSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IParameterSymbol parameterSymbol && Equals(parameterSymbol);
    }

    public override bool Equals(object? obj)
    {
        if (obj is ParameterSignature parameterSig) return Equals(parameterSig);
        if (obj is IParameterSymbol parameterSymbol) return Equals(parameterSymbol);
        if (obj is ParameterInfo parameterInfo) return Equals(parameterInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(SigType.Parameter, Name, ParameterType);
    }

    public override string ToString()
    {
        return $"{(IsParams ? "params " : "")}{ParameterType} {Name}{(HasDefaultValue ? $" = {DefaultValue}" : "")}";
    }
}