using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

namespace Jay
{
    public readonly struct EnumMemberInfo<TEnum> : IEquatable<EnumMemberInfo<TEnum>>,
                                                   IEquatable<TEnum> 
        where TEnum : unmanaged, Enum
    {
        public readonly TEnum Member;
        public readonly ulong Value;
        public readonly Attribute[] Attributes;
        public readonly string Name;

        public EnumMemberInfo(string name, TEnum member, ulong value, Attribute[] attributes)
        {
            this.Name = name;
            this.Member = member;
            this.Value = value;
            this.Attributes = attributes;
        }

        /// <inheritdoc />
        public bool Equals(EnumMemberInfo<TEnum> info)
        {
            return info.Value == this.Value;
        }

        /// <inheritdoc />
        public bool Equals(TEnum @enum)
        {
            return EnumInfo<TEnum>.Equals(Member, @enum);
        }
    }

   
    
    public static class EnumInfo<TEnum>
        where TEnum : unmanaged, Enum
    {
        private static readonly Type _enumType;
        private static readonly Attribute[] _attributes;
        private static readonly TEnum[] _members;
        private static readonly EnumMemberInfo<TEnum>[] _memberInfos;

        public static Type EnumType => _enumType;
        public static Attribute[] Attributes => _attributes;
        public static IReadOnlyList<TEnum> Members => _members;
        public static IReadOnlyList<EnumMemberInfo<TEnum>> MemberInfos => _memberInfos;
        
        static EnumInfo()
        {
            _enumType = typeof(TEnum);
            _attributes = Attribute.GetCustomAttributes(_enumType, true);
            var fields = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            var count = fields.Length;
            _members = new TEnum[count];
            _memberInfos = new EnumMemberInfo<TEnum>[count];
            for (var i = 0; i < count; i++)
            {
                var field = fields[i];
                // Slow path as this is cached
                TEnum member = (TEnum)field.GetValue(null)!;
                _members[i] = member;
                _memberInfos[i] = new EnumMemberInfo<TEnum>(field.Name, member, ToULong(member), 
                                                            Attribute.GetCustomAttributes(field));
            }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ToULong(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            Emit.Conv_U8();
            return Return<ulong>();
        }

        public static EnumMemberInfo<TEnum> GetInfo(TEnum @enum)
        {
            foreach (var info in _memberInfos)
            {
                if (Equals(info.Member, @enum))
                {
                    return info;
                }
            }
            throw new NotImplementedException();
        }

        public static bool TryParse(ReadOnlySpan<char> text, out TEnum @enum)
        {
            text = text.Trim();
            if (int.TryParse(text, out int integer))
            {
                foreach (var info in _memberInfos)
                {
                    if (info.Value == (ulong)integer)
                    {
                        @enum = info.Member;
                        return true;
                    }
                }
            }
            
            foreach (var info in _memberInfos)
            {
                if (MemoryExtensions.Equals(info.Name, text, StringComparison.OrdinalIgnoreCase))
                {
                    @enum = info.Member;
                    return true;
                }
            }

            @enum = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(TEnum x, TEnum y)
        {
            Emit.Ldarg(nameof(x));
            Emit.Ldarg(nameof(y));
            Emit.Ceq();
            return Return<bool>();
        }
    }
}