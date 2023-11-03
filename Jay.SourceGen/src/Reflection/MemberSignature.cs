// namespace Jay.SourceGen.Reflection;
//
// public abstract class MemberSig : Sig,
//     IEquatable<MemberSig>, IEquatable<ISymbol>, IEquatable<MemberInfo>
// {
//     [return: NotNullIfNotNull(nameof(memberInfo))]
//     public static implicit operator MemberSig?(MemberInfo? memberInfo) => MemberSig.Create(memberInfo);
//
//     public static bool operator ==(MemberSig? left, MemberSig? right) => FastEqual(left, right);
//     public static bool operator !=(MemberSig? left, MemberSig? right) => !FastEqual(left, right);
//     public static bool operator ==(MemberSig? left, ISymbol? right) => FastEquality(left, right);
//     public static bool operator !=(MemberSig? left, ISymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(MemberSig? left, MemberInfo? right) => FastEquality(left, right);
//     public static bool operator !=(MemberSig? left, MemberInfo? right) => !FastEquality(left, right);
//
//
//     [return: NotNullIfNotNull(nameof(symbol))]
//     public static MemberSig? Create(ISymbol? symbol)
//     {
//         switch (symbol)
//         {
//             case IFieldSymbol fieldSymbol:
//                 return new FieldSig(fieldSymbol);
//             case IPropertySymbol propertySymbol:
//                 return new PropertySig(propertySymbol);
//             case IEventSymbol eventSymbol:
//                 return new EventSig(eventSymbol);
//             case IMethodSymbol methodSymbol:
//                 return new MethodSig(methodSymbol);
//             case ITypeSymbol typeSymbol:
//                 return new TypeSig(typeSymbol);
//             default:
//                 return default;
//         }
//     }
//
//     [return: NotNullIfNotNull(nameof(member))]
//     public static MemberSig? Create(MemberInfo? member)
//     {
//         switch (member)
//         {
//             case FieldInfo fieldInfo:
//                 return new FieldSig(fieldInfo);
//             case PropertyInfo propertyInfo:
//                 return new PropertySig(propertyInfo);
//             case EventInfo eventInfo:
//                 return new EventSig(eventInfo);
//             case MethodBase methodBase:
//                 return new MethodSig(methodBase);
//             case Type type:
//                 return new TypeSig(type);
//             default:
//                 return default;
//         }
//     }
//
//     public static bool TryCreate([AllowNull, NotNullWhen(true)] object? obj, [NotNullWhen(true)] out MemberSig? memberSignature)
//     {
//         switch (obj)
//         {
//             case IFieldSymbol fieldSymbol:
//                 memberSignature = new FieldSig(fieldSymbol);
//                 return true;
//             case FieldInfo fieldInfo:
//                 memberSignature = new FieldSig(fieldInfo);
//                 return true;
//             case IPropertySymbol propertySymbol:
//                 memberSignature = new PropertySig(propertySymbol);
//                 return true;
//             case PropertyInfo propertyInfo:
//                 memberSignature = new PropertySig(propertyInfo);
//                 return true;
//             case IEventSymbol eventSymbol:
//                 memberSignature = new EventSig(eventSymbol);
//                 return true;
//             case EventInfo eventInfo:
//                 memberSignature = new EventSig(eventInfo);
//                 return true;
//             case IMethodSymbol methodSymbol:
//                 memberSignature = new MethodSig(methodSymbol);
//                 return true;
//             case MethodBase methodBase:
//                 memberSignature = new MethodSig(methodBase);
//                 return true;
//             case Type type:
//                 memberSignature = new TypeSig(type);
//                 return true;
//             default:
//                 memberSignature = default;
//                 return false;
//         }
//     }
//
//
//     public TypeSig? ParentType { get; init; }
//     public SignatureAttributes Attributes { get; init; }
//
//     protected MemberSig(ISymbol symbol) : base(symbol.Name, symbol.GetVisibility(), symbol.GetKeywords())
//     {
//         this.ParentType = TypeSig.Create(symbol.ContainingType);
//         this.Attributes = new(symbol);
//     }
//
//     protected MemberSig(MemberInfo member) : base(member.Name, member.Visibility(), member.GetKeywords())
//     {
//         this.ParentType = TypeSig.Create(member.ReflectedType ?? member.DeclaringType);
//         this.Attributes = new(member);
//     }
//
//
//     public override bool Equals(Sig? signature)
//     {
//         return signature is MemberSig memberSig && Equals(memberSig);
//     }
//
//     public virtual bool Equals(MemberSig? memberSig)
//     {
//         return base.Equals(memberSig)
//             && FastEqual(ParentType, memberSig.ParentType)
//             && FastEqual(Attributes, memberSig.Attributes);
//     }
//
//     public virtual bool Equals(MemberInfo? memberInfo)
//     {
//         return Equals(Create(memberInfo));
//     }
//
//     public virtual bool Equals(ISymbol? symbol)
//     {
//         return Equals(Create(symbol));
//     }
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             MemberSig memberSig => Equals(memberSig),
//             ISymbol symbol => Equals(symbol),
//             MemberInfo member => Equals(member),
//             _ => false
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(base.GetHashCode(), ParentType, Attributes);
//     }
// }