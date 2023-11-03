// using System.Runtime.CompilerServices;
// using static Jay.StaticImports;
//
// namespace Jay.SourceGen.Reflection;
//
// public sealed class ParameterSig :
//     Sig,
//     IEquatable<ParameterSig>,
//     IEquatable<IParameterSymbol>,
//     IEquatable<ParameterInfo>
// {
//     [return: NotNullIfNotNull(nameof(parameterInfo))]
//     public static implicit operator ParameterSig?(ParameterInfo? parameterInfo) => Create(parameterInfo);
//
//     public static bool operator ==(ParameterSig? left, ParameterSig? right) => FastEqual(left, right);
//     public static bool operator !=(ParameterSig? left, ParameterSig? right) => !FastEqual(left, right);
//     public static bool operator ==(ParameterSig? left, IParameterSymbol? right) => FastEquality(left, right);
//     public static bool operator !=(ParameterSig? left, IParameterSymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(ParameterSig? left, ParameterInfo? right) => FastEquality(left, right);
//     public static bool operator !=(ParameterSig? left, ParameterInfo? right) => !FastEquality(left, right);
//
//     [return: NotNullIfNotNull(nameof(parameterSymbol))]
//     public static ParameterSig? Create(IParameterSymbol? parameterSymbol)
//     {
//         if (parameterSymbol is null)
//             return null;
//
//         return new ParameterSig(parameterSymbol);
//     }
//
//     [return: NotNullIfNotNull(nameof(parameterInfo))]
//     public static ParameterSig? Create(ParameterInfo? parameterInfo)
//     {
//         if (parameterInfo is null)
//             return null;
//
//         return new ParameterSig(parameterInfo);
//     }
//
//     public RefKind RefKind { get; }
//     public TypeSig? ParameterType { get; }
//     public Option<object?> DefaultValue { get; }
//     public bool IsThis { get; }
//     public bool IsParams { get; }
//
//     public ParameterSig(IParameterSymbol parameterSymbol)
//         : base(parameterSymbol.Name, parameterSymbol.GetVisibility(), parameterSymbol.GetKeywords())
//     {
//         this.RefKind = parameterSymbol.RefKind;
//         this.ParameterType = new TypeSig(parameterSymbol.Type);
//         if (parameterSymbol.HasExplicitDefaultValue)
//         {
//             this.DefaultValue = Some<object?>(parameterSymbol.ExplicitDefaultValue);
//         }
//         else
//         {
//             this.DefaultValue = None<object?>();
//         }
//         this.IsThis = parameterSymbol.IsThis;
//         this.IsParams = parameterSymbol.IsParams;
//     }
//
//     public ParameterSig(ParameterInfo parameterInfo)
//         : base(parameterInfo.Name, default, default)
//     {
//         var access = parameterInfo.GetAccess(out var parameterType);
//         this.RefKind = access switch
//         {
//             ParameterAccess.Default => RefKind.None,
//             ParameterAccess.In => RefKind.In,
//             ParameterAccess.Ref => RefKind.Ref,
//             ParameterAccess.Out => RefKind.Out,
//             _ => throw new ArgumentOutOfRangeException(),
//         };
//         this.ParameterType = new TypeSig(parameterType);
//         if (parameterInfo.HasDefaultValue)
//         {
//             this.DefaultValue = Some<object?>(parameterInfo.DefaultValue);
//         }
//         else
//         {
//             this.DefaultValue = None<object?>();
//         }
//         this.IsThis = parameterInfo.Member.HasAttribute<ExtensionAttribute>();
//         this.IsParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
//     }
//
//     public bool Equals(ParameterSig? parameterSig)
//     {
//         return base.Equals(parameterSig)
//             && FastEqual(ParameterType, parameterSig.ParameterType)
//             && FastEqual(DefaultValue, parameterSig.DefaultValue)
//             && FastEqual(IsParams, parameterSig.IsParams);
//     }
//
//     public override bool Equals(Sig? signature)
//     {
//         return signature is ParameterSig parameterSig && Equals(parameterSig);
//     }
//
//     public bool Equals(IParameterSymbol? parameterSymbol) => Equals(Create(parameterSymbol));
//
//     public bool Equals(ParameterInfo? parameterInfo) => Equals(Create(parameterInfo));
//     
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             ParameterSig parameterSig => Equals(parameterSig),
//             IParameterSymbol parameterSymbol => Equals(parameterSymbol),
//             ParameterInfo parameterInfo => Equals(parameterInfo),
//             _ => false
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(base.GetHashCode(), ParameterType, DefaultValue, IsParams);
//     }
//
//     public override string ToString()
//     {
//         using var code = new TextBuilder();
//         if (IsThis)
//             code.Write("this ");
//         switch (RefKind)
//         {
//             case RefKind.Ref:
//                 code.Write("ref ");
//                 break;
//             case RefKind.Out:
//                 code.Write("out ");
//                 break;
//             case RefKind.In:
//                 code.Write("in ");
//                 break;
//         }
//         if (IsParams)
//             code.Write("params ");
//         code.Append(ParameterType);
//         if (DefaultValue.IsSome(out var defaultValue))
//         {
//             code.Append(" = ")
//                 .Append(defaultValue);
//         }
//         return code.ToString();
//     }
// }