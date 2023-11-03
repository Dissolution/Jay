// using Jay.SourceGen.Collections;
//
// namespace Jay.SourceGen.Reflection;
//
// public sealed class AttributeSig : Sig,
//         IEquatable<AttributeSig>,
//         IEquatable<AttributeData>,
//         IEquatable<CustomAttributeData>
// {
//     [return: NotNullIfNotNull(nameof(attributeData))]
//     public static implicit operator AttributeSig?(AttributeData? attributeData) =>
//         Create(attributeData);
//
//     [return: NotNullIfNotNull(nameof(customAttrData))]
//     public static implicit operator AttributeSig?(CustomAttributeData? customAttrData) =>
//         Create(customAttrData);
//
//     public static bool operator ==(AttributeSig? left, AttributeSig? right) => FastEqual(left, right);
//     public static bool operator !=(AttributeSig? left, AttributeSig? right) => !FastEqual(left, right);
//     public static bool operator ==(AttributeSig? left, AttributeData? right) => FastEquality(left, right);
//     public static bool operator !=(AttributeSig? left, AttributeData? right) => !FastEquality(left, right);
//     public static bool operator ==(AttributeSig? left, CustomAttributeData? right) => FastEquality(left, right);
//     public static bool operator !=(AttributeSig? left, CustomAttributeData? right) => !FastEquality(left, right);
//
//     [return: NotNullIfNotNull(nameof(attributeData))]
//     public static AttributeSig? Create(AttributeData? attributeData)
//     {
//         if (attributeData is null)
//             return null;
//         return new AttributeSig(attributeData);
//     }
//
//     [return: NotNullIfNotNull(nameof(customAttrData))]
//     public static AttributeSig? Create(CustomAttributeData? customAttrData)
//     {
//         if (customAttrData is null)
//             return null;
//         return new AttributeSig(customAttrData);
//     }
//     
//     public TypeSig? AttributeType { get; }
//     public AttributeArguments Arguments { get; }
//
//     public AttributeSig(AttributeData attributeData)
//         : base(attributeData.AttributeClass?.Name ?? attributeData.ToString(), default, default)
//     {
//         this.AttributeType = TypeSig.Create(attributeData.AttributeClass);
//         this.Arguments = new(attributeData);
//     }
//
//     public AttributeSig(CustomAttributeData customAttributeData)
//         : base(customAttributeData.AttributeType.Name, default, default)
//     {
//         this.AttributeType = TypeSig.Create(customAttributeData.AttributeType);
//         this.Arguments = new(customAttributeData);
//     }
//
//     public override bool Equals(Sig? signature)
//     {
//         return signature is AttributeSig attributeSig && Equals(attributeSig);
//     }
//     
//     public bool Equals(AttributeSig? attributeSig)
//     {
//         return attributeSig is not null
//             && FastEqual(AttributeType, attributeSig.AttributeType)
//             && TextEqual(Name, attributeSig.Name)
//             && SetEqual(Arguments, attributeSig.Arguments);
//     }
//
//     public bool Equals(AttributeData? attributeData) => Equals(Create(attributeData));
//
//     public bool Equals(CustomAttributeData? customAttrData) => Equals(Create(customAttrData));
//     
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             AttributeSig attributeSig => Equals(attributeSig),
//             AttributeData attributeData => Equals(attributeData),
//             CustomAttributeData customAttrData => Equals(customAttrData),
//             _ => false,
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(AttributeType, Name, Arguments);
//     }
//
//     public override string ToString()
//     {
//         using var code = new TextBuilder();
//         code.Write(Name);
//         if (Arguments.Count > 0)
//         {
//             code.Append('(')
//                 .Delimit(static c => c.Write(", "), Arguments, static (cb, a) => cb.Append(a.Key).Append(" = ").Append(a.Value))
//                 .Write(')');
//         }
//         return code.ToString();
//     }
// }
