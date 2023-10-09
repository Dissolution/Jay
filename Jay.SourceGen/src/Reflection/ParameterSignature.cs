using System.Runtime.CompilerServices;
using static Jay.StaticImports;

namespace Jay.SourceGen.Reflection;

public sealed class ParameterSignature :
    Signature,
    IEquatable<ParameterSignature>,
    IEquatable<IParameterSymbol>,
    IEquatable<ParameterInfo>
{
    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static implicit operator ParameterSignature?(ParameterInfo? parameterInfo) => Create(parameterInfo);

    public static bool operator ==(ParameterSignature? left, ParameterSignature? right) => FastEqual(left, right);
    public static bool operator !=(ParameterSignature? left, ParameterSignature? right) => !FastEqual(left, right);
    public static bool operator ==(ParameterSignature? left, IParameterSymbol? right) => FastEquality(left, right);
    public static bool operator !=(ParameterSignature? left, IParameterSymbol? right) => !FastEquality(left, right);
    public static bool operator ==(ParameterSignature? left, ParameterInfo? right) => FastEquality(left, right);
    public static bool operator !=(ParameterSignature? left, ParameterInfo? right) => !FastEquality(left, right);

    [return: NotNullIfNotNull(nameof(parameterSymbol))]
    public static ParameterSignature? Create(IParameterSymbol? parameterSymbol)
    {
        if (parameterSymbol is null)
            return null;

        return new ParameterSignature(parameterSymbol);
    }

    [return: NotNullIfNotNull(nameof(parameterInfo))]
    public static ParameterSignature? Create(ParameterInfo? parameterInfo)
    {
        if (parameterInfo is null)
            return null;

        return new ParameterSignature(parameterInfo);
    }

    public RefKind RefKind { get; }
    public TypeSignature? ParameterType { get; }
    public Option<object?> DefaultValue { get; }
    public bool IsThis { get; }
    public bool IsParams { get; }

    public ParameterSignature(IParameterSymbol parameterSymbol)
        : base(parameterSymbol.Name, parameterSymbol.GetVisibility(), parameterSymbol.GetKeywords())
    {
        this.RefKind = parameterSymbol.RefKind;
        this.ParameterType = new TypeSignature(parameterSymbol.Type);
        if (parameterSymbol.HasExplicitDefaultValue)
        {
            this.DefaultValue = Some<object?>(parameterSymbol.ExplicitDefaultValue);
        }
        else
        {
            this.DefaultValue = None<object?>();
        }
        this.IsThis = parameterSymbol.IsThis;
        this.IsParams = parameterSymbol.IsParams;
    }

    public ParameterSignature(ParameterInfo parameterInfo)
        : base(parameterInfo.Name, default, default)
    {
        var access = parameterInfo.GetAccess(out var parameterType);
        this.RefKind = access switch
        {
            ParameterAccess.Default => RefKind.None,
            ParameterAccess.In => RefKind.In,
            ParameterAccess.Ref => RefKind.Ref,
            ParameterAccess.Out => RefKind.Out,
            _ => throw new ArgumentOutOfRangeException(),
        };
        this.ParameterType = new TypeSignature(parameterType);
        if (parameterInfo.HasDefaultValue)
        {
            this.DefaultValue = Some<object?>(parameterInfo.DefaultValue);
        }
        else
        {
            this.DefaultValue = None<object?>();
        }
        this.IsThis = parameterInfo.Member.HasAttribute<ExtensionAttribute>();
        this.IsParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
    }

    public bool Equals(ParameterSignature? parameterSig)
    {
        return base.Equals(parameterSig)
            && FastEqual(ParameterType, parameterSig.ParameterType)
            && FastEqual(DefaultValue, parameterSig.DefaultValue)
            && FastEqual(IsParams, parameterSig.IsParams);
    }

    public override bool Equals(Signature? signature)
    {
        return signature is ParameterSignature parameterSig && Equals(parameterSig);
    }

    public bool Equals(IParameterSymbol? parameterSymbol) => Equals(Create(parameterSymbol));

    public bool Equals(ParameterInfo? parameterInfo) => Equals(Create(parameterInfo));
    
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            ParameterSignature parameterSig => Equals(parameterSig),
            IParameterSymbol parameterSymbol => Equals(parameterSymbol),
            ParameterInfo parameterInfo => Equals(parameterInfo),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(base.GetHashCode(), ParameterType, DefaultValue, IsParams);
    }

    public override void DeclareTo(CodeBuilder code)
    {
        if (IsThis)
            code.Write("this ");
        switch (RefKind)
        {
            case RefKind.Ref:
                code.Write("ref ");
                break;
            case RefKind.Out:
                code.Write("out ");
                break;
            case RefKind.In:
                code.Write("in ");
                break;
        }
        if (IsParams)
            code.Write("params ");
        code.Code(ParameterType);
        if (DefaultValue.IsSome(out var defaultValue))
        {
            code.Write(" = ")
                .Code(defaultValue);
        }
    }
}