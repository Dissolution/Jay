// using Jay.Reflection.Adapters;
//
// namespace Jay.Reflection.Enums;
//
// public abstract class EnumTypeInfo : 
//     IEquatable<EnumTypeInfo>, IEquatable<Type>,
//     ICodePart
// {
//     protected readonly EnumMemberInfo[] _enumMemberInfos;
//     
//     public Type EnumType { get; }
//     public string Name { get; }
//     public IReadOnlyList<Attribute> Attributes { get; }
//     public bool IsFlags { get; }
//     public IReadOnlyList<EnumMemberInfo> Members => _enumMemberInfos;
//
//     public EnumTypeInfo(Type enumType)
//     {
//         if (!enumType.IsEnum || !enumType.IsValueType)
//             throw ReflectedException.Create<ArgumentException>(
//                 $"{enumType} is not a valid enum type", 
//                 nameof(enumType));
//         this.EnumType = enumType;
//         this.Name = enumType.Name;
//         this.Attributes = Attribute.GetCustomAttributes(enumType);
//         this.IsFlags = Attributes.OfType<FlagsAttribute>().Any();
//         _enumMemberInfos = GetEnumMemberInfos();
//     }
//
//     protected abstract EnumMemberInfo[] GetEnumMemberInfos();
//     
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     protected int FindIndex(ulong value)
//     {
//         // Values are sorted ascending
//         int index = Array.BinarySearch(_enumMemberInfos, value);
//         // Normalize to -1 as a failed match
//         if ((uint)index > _memberCount)
//             return -1;
//         return index;
//     }
// }
//
// public class EnumTypeInfo<TEnum> : EnumTypeInfo,
//     IEquatable<EnumTypeInfo<TEnum>>
//     where TEnum : struct, Enum
// {
//     public static EnumComparer<TEnum> Comparer { get; } = new();
//     
//     public new IReadOnlyList<EnumMemberInfo<TEnum>> Members { get; }
//
//     public EnumTypeInfo() : base(typeof(TEnum))
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
//     
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