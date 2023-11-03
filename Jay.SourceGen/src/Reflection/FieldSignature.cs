// namespace Jay.SourceGen.Reflection;
//
// public sealed class FieldSig : MemberSig,
//     IEquatable<FieldSig>, IEquatable<IFieldSymbol>, IEquatable<FieldInfo>
// {
//     [return: NotNullIfNotNull(nameof(fieldInfo))]
//     public static implicit operator FieldSig?(FieldInfo? fieldInfo) => Create(fieldInfo);
//     
//     public static bool operator ==(FieldSig? left, FieldSig? right) => FastEqual(left, right);
//     public static bool operator !=(FieldSig? left, FieldSig? right) => !FastEqual(left, right);
//     public static bool operator ==(FieldSig? left, IFieldSymbol? right) => FastEquality(left, right);
//     public static bool operator !=(FieldSig? left, IFieldSymbol? right) => !FastEquality(left, right);
//     public static bool operator ==(FieldSig? left, FieldInfo? right) => FastEquality(left, right);
//     public static bool operator !=(FieldSig? left, FieldInfo? right) => !FastEquality(left, right);
//     
//     
//     [return: NotNullIfNotNull(nameof(fieldSymbol))]
//     public static FieldSig? Create(IFieldSymbol? fieldSymbol)
//     {
//         if (fieldSymbol is null) return null;
//         return new FieldSig(fieldSymbol);
//     }
//     [return: NotNullIfNotNull(nameof(fieldInfo))]
//     public static FieldSig? Create(FieldInfo? fieldInfo)
//     {
//         if (fieldInfo is null) return null;
//         return new FieldSig(fieldInfo);
//     }
//
//     public TypeSig FieldType { get; init; }
//     
//     internal FieldSig(IFieldSymbol fieldSymbol)
//         : base(fieldSymbol)
//     {
//         this.FieldType = TypeSig.Create(fieldSymbol.Type);
//     }
//
//     internal FieldSig(FieldInfo fieldInfo)
//         : base(fieldInfo)
//     {
//         this.FieldType = TypeSig.Create(fieldInfo.FieldType);
//     }
//
//     public bool Equals(FieldSig? fieldSig)
//     {
//         return base.Equals(fieldSig)
//             && FastEqual(FieldType, fieldSig.FieldType);
//     }
//     
//     public override bool Equals(MemberSig? memberSig)
//     {
//         return memberSig is FieldSig fieldSig && Equals(fieldSig);
//     }
//     
//     public override bool Equals(Sig? signature)
//     {
//         return signature is FieldSig fieldSig && Equals(fieldSig);
//     }
//
//     public bool Equals(IFieldSymbol? fieldSymbol)
//     {
//         return Equals(Create(fieldSymbol));
//     }
//     
//     public override bool Equals(ISymbol? symbol)
//     {
//         return symbol is IFieldSymbol fieldSymbol && Equals(fieldSymbol);
//     }
//
//     public bool Equals(FieldInfo? fieldInfo)
//     {
//         return Equals(Create(fieldInfo));
//     }
//     
//     public override bool Equals(MemberInfo? memberInfo)
//     {
//         return memberInfo is FieldInfo fieldInfo && Equals(fieldInfo);
//     }
//
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             FieldSig fieldSig => Equals(fieldSig),
//             IFieldSymbol fieldSymbol => Equals(fieldSymbol),
//             FieldInfo fieldInfo => Equals(fieldInfo),
//             _ => false
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(GetType(), Name, Visibility, Keywords, FieldType);
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
//             .Append(FieldType)
//             .Append(' ')
//             .Append(Name)
//             .Append(';')
//             .ToStringAndDispose();
//     }
// }
