// namespace Jay.SourceGen.Reflection;
//
// public sealed class MethodSig :
//     MemberSig,
//     IEquatable<MethodSig>,
//     IEquatable<IMethodSymbol>,
//     IEquatable<MethodBase>
// {
//     [return: NotNullIfNotNull(nameof(methodBase))]
//     public static implicit operator MethodSig?(MethodBase? methodBase) => Create(methodBase);
//
//
//     public static bool operator ==(MethodSig? left, MethodSig? right) => FastEqual(left, right);
//     public static bool operator !=(MethodSig? left, MethodSig? right) => !FastEqual(left, right);
//     public static bool operator ==(MethodSig? left, IMethodSymbol? right) => FastEquality(left, right);
//     public static bool operator !=(MethodSig? left, IMethodSymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(MethodSig? left, MethodBase? right) => FastEquality(left, right);
//     public static bool operator !=(MethodSig? left, MethodBase? right) => !FastEquality(left, right);
//
//     public static MethodSig? Create(IMethodSymbol? methodSymbol)
//     {
//         if (methodSymbol is null)
//             return null;
//
//         return new MethodSig(methodSymbol);
//     }
//
//     public static MethodSig? Create(MethodBase? methodBase)
//     {
//         if (methodBase is null)
//             return null;
//
//         return new MethodSig(methodBase);
//     }
//
//
//     public TypeSig? ReturnType { get; }
//
//     public IReadOnlyList<ParameterSig> Parameters { get; }
//
//     public MethodSig(IMethodSymbol methodSymbol)
//         : base(methodSymbol)
//     {
//         this.ReturnType = TypeSig.Create(methodSymbol.ReturnType);
//         this.Parameters = methodSymbol.Parameters
//             .Select(static p => new ParameterSig(p))
//             .ToList();
//     }
//
//     public MethodSig(MethodBase methodBase)
//         : base(methodBase)
//     {
//         this.ReturnType = TypeSig.Create(methodBase.ReturnType());
//         this.Parameters = methodBase.GetParameters()
//             .Select(static p => new ParameterSig(p))
//             .ToList();
//     }
//
//     public bool Equals(MethodSig? methodSig)
//     {
//         return base.Equals(methodSig)
//             && FastEqual(ReturnType, methodSig.ReturnType)
//             && SeqEqual(Parameters, methodSig.Parameters);
//     }
//     
//     public override bool Equals(MemberSig? memberSig)
//     {
//         return memberSig is MethodSig methodSig && Equals(methodSig);
//     }
//     
//     public override bool Equals(Sig? signature)
//     {
//         return signature is MethodSig methodSig && Equals(methodSig);
//     }
//
//     public bool Equals(IMethodSymbol? methodSymbol) => Equals(Create(methodSymbol));
//
//     public bool Equals(MethodBase? methodBase) => Equals(Create(methodBase));
//
//     public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));
//
//     public override bool Equals(MemberInfo? memberInfo) => Equals(Create(memberInfo));
//     
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             MethodSig methodSig => Equals(methodSig),
//             IMethodSymbol methodSymbol => Equals(methodSymbol),
//             MethodBase methodBase => Equals(methodBase),
//             _ => false
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         Hasher hasher = new();
//         hasher.Add(base.GetHashCode());
//         hasher.Add(ReturnType);
//         hasher.AddAll<ParameterSig>(Parameters);
//         return hasher.ToHashCode();
//     }
//
//     public override string ToString()
//     {
//         return TextBuilder.New
//             .Append(Attributes)
//             .Append(Visibility)
//             .Append(' ')
//             .Append(Keywords)
//             .Append(' ')
//             .Append(ReturnType)
//             .Append(' ')
//             .Append(Name)
//             .Append('(')
//             .Delimit(static cb => cb.Write(", "), Parameters, static (cb, p) => cb.Write(p))
//             .Append(");")
//             .ToStringAndDispose();
//     }
// }