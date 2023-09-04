// using Jay.Reflection.Adapters;
// using Jay.Reflection.Exceptions;
// using Jay.Reflection.Validation;
//
// namespace Jay.Reflection.Enums;
//
// public abstract class EnumTypeInfo : 
//     IEquatable<EnumTypeInfo>,
//     ICodePart
// {
//     protected readonly EnumMemberInfo[] _members;
//     
//     public Type EnumType { get; }
//
//     public Type EnumUnderlyingType => Enum.GetUnderlyingType(this.EnumType);
//     public string Name { get; }
//     public IReadOnlyCollection<Attribute> Attributes { get; }
//     public bool IsFlags { get; }
//     public IReadOnlyCollection<EnumMemberInfo> Members => _members;
//
//     protected EnumTypeInfo(Type enumType, EnumMemberInfo[] members)
//     {
//         ValidateType.IsEnum(enumType);
//         this.EnumType = enumType;
//         this.Name = enumType.Name;
//         this.Attributes = Attribute.GetCustomAttributes(enumType);
//         this.IsFlags = Attributes.OfType<FlagsAttribute>().Any();
//         _members = members;
//     }
//
//     public bool Equals(EnumTypeInfo? enumTypeInfo)
//     {
//         return this.EnumType == enumTypeInfo?.EnumType;
//     }
//
//     public bool Equals(Type? enumType)
//     {
//         return this.EnumType == enumType;
//     }
//
//     public override bool Equals(object? obj)
//     {
//         if (obj is EnumTypeInfo enumTypeInfo)
//             return Equals(enumTypeInfo);
//         if (obj is Type enumType)
//             return Equals(enumType);
//         return false;
//     }
//
//     public override int GetHashCode()
//     {
//         return this.EnumType.GetHashCode();
//     }
//
//     public void DeclareTo(CodeBuilder codeBuilder)
//     {
//         codeBuilder.Write(this.Name);
//     }
//
//     public override string ToString() => CodePart.ToDeclaration(this);
// }
//
// public sealed class EnumTypeInfo<TEnum> : EnumTypeInfo,
//     IEquatable<EnumTypeInfo<TEnum>>
//     where TEnum : struct, Enum
// {
//     public new IReadOnlyList<EnumMemberInfo<TEnum>> Members { get; }
//
//     public EnumTypeInfo() 
//         : base(typeof(TEnum))
//     {
//         var enumFields = this.EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
//         int count = enumFields.Length;
//         var members = new EnumMemberInfo<TEnum>[count];
//         for (var i = 0; i < count; i++)
//         {
//             members[i] = new EnumMemberInfo<TEnum>(enumFields[i]);
//         }
//         // Ensure that we are in value order (for binary search)
//         Array.Sort(members, (l,r) => l.UInt64Value.CompareTo(r.UInt64Value));
//         this.Members = members;
//     }
//
//     private EnumMemberInfo<TEnum> GetMembers()
//     {
//         
//     }
// }
//
// public class EnumMemberInfo : 
//     IEquatable<EnumMemberInfo>, IEquatable<Enum>,
//     IComparable<EnumMemberInfo>, IComparable<Enum>
// {
//     public string Name { get; }
//     internal FieldInfo Field { get; }
//     public ulong UInt64Value { get; }
//     public IReadOnlyList<Attribute> Attributes { get; }
//     
//     public EnumMemberInfo(FieldInfo enumMemberField)
//     {
//         this.Name = enumMemberField.Name;
//         this.Field = enumMemberField;
//         this.UInt64Value = (ulong)enumMemberField.GetValue(null)!;
//         this.Attributes = Attribute.GetCustomAttributes(enumMemberField);
//     }
// }
//
// public class EnumMemberInfo<TEnum> : EnumMemberInfo,
//     IEquatable<EnumMemberInfo<TEnum>>, IEquatable<TEnum>
//     where TEnum : struct, Enum
// {
//     public TEnum Member { get; }
//     
//     public EnumMemberInfo(FieldInfo enumMemberField)
//         : base(enumMemberField)
//     {
//         this.Member = (TEnum)enumMemberField.GetValue(null)!;
//     }
// }