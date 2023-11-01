// using System.Diagnostics;
//
// namespace Jay.Reflection.Enums;
//
// public static class SmartEnum
// {
//     public static bool TryParse<TEnum>([NotNullWhen(true)] string? str, out TEnum @enum)
//         where TEnum : struct, Enum
//     {
//         throw new NotImplementedException();
//     }
// }
//
//
// public abstract class EnumTypeInfo
// {
//     protected readonly EnumMemberInfo[] _members;
//     
//     public Type Type { get; }
//     public string Name { get; }
//     public IReadOnlyList<Attribute> Attributes { get; }
//     public bool IsFlags { get; }
//     public IReadOnlyList<EnumMemberInfo> Members => _members;
//
//     protected EnumTypeInfo(Type enumType)
//     {
//         Debug.Assert(enumType is not null);
//         Debug.Assert(enumType.IsValueType);
//         Debug.Assert(enumType.IsEnum);
//         this.Type = enumType;
//         this.Name = enumType.Name;
//         this.Attributes = Attribute.GetCustomAttributes(enumType);
//         this.IsFlags = Attributes.OfType<FlagsAttribute>().Any();
//
//         // ReSharper disable once VirtualMemberCallInConstructor
//         _members = FindMembers();
//     }
//
//     protected abstract EnumMemberInfo[] FindMembers();
// }
//
// public sealed class EnumTypeInfo<E> : EnumTypeInfo
//     where E : struct, Enum
// {
//     public new IReadOnlyList<EnumMemberInfo<E>> Members { get; }
//
//     public EnumTypeInfo() : base(typeof(E))
//     {
//         
//     }
//
//     protected override EnumMemberInfo[] FindMembers()
//     {
//         var memberFields = this.Type.GetFields(BindingFlags.Public | BindingFlags.Static);
//         int count = memberFields.Length;
//         var members = new EnumMemberInfo<E>[count];
//         for (var i = 0; i < count; i++)
//         {
//             members[i] = new EnumMemberInfo<E>(this, memberFields[i]);
//         }
//         // ReSharper disable once CoVariantArrayConversion
//         return members;
//     }
// }
//
// public abstract class EnumMemberInfo
// {
//     protected readonly FieldInfo _memberField;
//     
//     public EnumTypeInfo TypeInfo { get; }
//     public string Name { get; }
//     public IReadOnlyList<Attribute> Attributes { get; }
//     
//     protected EnumMemberInfo(EnumTypeInfo enumTypeInfo, FieldInfo memberField)
//     {
//         this.TypeInfo = enumTypeInfo;
//         _memberField = memberField;
//         this.Name = memberField.Name;
//         this.Attributes = Attribute.GetCustomAttributes(memberField);
//     }
// }
//
// public sealed class EnumMemberInfo<E> : EnumMemberInfo
//     where E : struct, Enum
// {
//     public new EnumTypeInfo<E> TypeInfo { get; }
//     public E Enum { get; }
//     public ulong UInt64Value { get; }
//
//     internal EnumMemberInfo(EnumTypeInfo<E> enumTypeInfo, FieldInfo memberField)
//         : base(enumTypeInfo, memberField)
//     {
//         this.TypeInfo = enumTypeInfo;
//         this.Enum = (E)memberField.GetValue(default)!;
//         this.UInt64Value = Enum.ToUInt64();
//     }
// }