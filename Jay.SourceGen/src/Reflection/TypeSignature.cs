// namespace Jay.SourceGen.Reflection;
//
// public sealed class TypeSig : MemberSig,
//     IEquatable<TypeSig>, IEquatable<ITypeSymbol>, IEquatable<Type>
// {
//     [return: NotNullIfNotNull(nameof(type))]
//     public static implicit operator TypeSig?(Type? type) => type is null ? null : new TypeSig(type);
//
//     public static bool operator ==(TypeSig? left, TypeSig? right) => FastEqual(left, right);
//     public static bool operator !=(TypeSig? left, TypeSig? right) => !FastEqual(left, right);
//     public static bool operator ==(TypeSig? left, ITypeSymbol? right) => FastEquality(left, right);
//     public static bool operator !=(TypeSig? left, ITypeSymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(TypeSig? left, Type? right) => FastEquality(left, right);
//     public static bool operator !=(TypeSig? left, Type? right) => !FastEquality(left, right);
//
//     [return: NotNullIfNotNull(nameof(type))]
//     public static TypeSig? Create(Type? type)
//     {
//         if (type is null)
//             return null;
//         return new TypeSig(type);
//     }
//     
//     [return: NotNullIfNotNull(nameof(typeSymbol))]
//     public static TypeSig? Create(ITypeSymbol? typeSymbol)
//     {
//         if (typeSymbol is null)
//             return null;
//         return new TypeSig(typeSymbol);
//     }
//
//     public string? Namespace { get; init; } = null;
//     public string? FullName { get; init; } = null;
//     public TypeKind Kind { get; init; } = default;
//
//     public TypeSig(ITypeSymbol typeSymbol)
//         : base(typeSymbol)
//     {
//         this.Namespace = typeSymbol.GetNamespace();
//         this.FullName = typeSymbol.GetFullName();
//         this.Kind = typeSymbol.TypeKind;
//     }
//     
//
//     public TypeSig(Type type)
//         : base(type)
//     {
//         this.Namespace = type.Namespace;
//         this.FullName = type.FullName;
//         if (type.IsValueType)
//         {
//             Kind = TypeKind.Struct;
//         }
//         else if (typeof(Delegate).IsAssignableFrom(type))
//         {
//             Kind = TypeKind.Delegate;
//         }
//         else if (type.IsInterface)
//         {
//             Kind = TypeKind.Interface;
//         }
//         else if (type.IsClass)
//         {
//             Kind = TypeKind.Class;
//         }
//         else
//             throw new NotImplementedException();
//     }
//
//     public bool Equals(TypeSig? typeSig)
//     {
//         return typeSig is not null && string.Equals(FullName, typeSig.FullName);
//     }
//
//     public override bool Equals(MemberSig? memberSig)
//     {
//         return memberSig is TypeSig typeSig && Equals(typeSig);
//     }
//     
//     public override bool Equals(Sig? signature)
//     {
//         return signature is TypeSig typeSig && Equals(typeSig);
//     }
//
//     public bool Equals(ITypeSymbol? typeSymbol) => Equals(Create(typeSymbol));
//
//     public override bool Equals(ISymbol? symbol) => Equals(Create(symbol));
//
//     public bool Equals(Type? type) => Equals(Create(type));
//
//     public override bool Equals(MemberInfo? member) => Equals(Create(member));
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             TypeSig typeSig => Equals(typeSig),
//             ITypeSymbol typeSymbol => Equals(typeSymbol),
//             Type type => Equals(type),
//             _ => false,
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(base.GetHashCode(), Kind, Namespace);
//     }
//
//     public override string ToString()
//     {
//         return Name;
//     }
// }