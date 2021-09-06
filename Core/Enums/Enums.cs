using InlineIL;
using Jay.Comparison;
using Jay.Reflection;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using static InlineIL.IL;
using static InlineIL.IL.Emit;

namespace Jay
{
    public static class Enums
    {
        static Enums()
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag<TEnum>(this TEnum e, TEnum flag)
            where TEnum : struct, Enum =>
            Enums<TEnum>.HasFlag(e, flag);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTheFlag<TEnum>(this TEnum e, TEnum flag)
            where TEnum : struct, Enum =>
            Enums<TEnum>.HasFlag(e, flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LacksFlag<TEnum>(this TEnum e, TEnum flag)
            where TEnum : struct, Enum =>
            !Enums<TEnum>.HasFlag(e, flag);
        
        public static TEnum HighestFlag<TEnum>(this TEnum e)
            where TEnum : struct, Enum
        {
            ulong value = Enums<TEnum>.GetValue(e);
            ulong mask = 1UL << 63;
            for (var i = 0; i < 63; i++)
            {
                // HasFlag: (e & flag) == flag
                if ((value & mask) == mask)
                    return Enums<TEnum>.GetMember(mask);
                mask = mask >> 1;
            }
            return default;
        }
        
        public static TEnum LowestFlag<TEnum>(this TEnum e)
            where TEnum : struct, Enum
        {
            ulong value = Enums<TEnum>.GetValue(e);
            ulong mask = 1UL << 0;
            for (var i = 0; i < 63; i++)
            {
                // HasFlag: (e & flag) == flag
                if ((value & mask) == mask)
                    return Enums<TEnum>.GetMember(mask);
                mask = mask << 1;
            }
            return default;
        }
    }

    public static class Enums<TEnum>
        where TEnum : struct, Enum
    {
        internal sealed class EnumData
        {
            private readonly TEnum _enum;
            private readonly ulong _value;
            private readonly string? _name;
            private readonly Attribute[] _attributes;

            public TEnum Enum => _enum;
            public string Name => _name ?? string.Empty;
            public IReadOnlyList<Attribute> Attributes => _attributes;

            public EnumData(TEnum @enum, ulong value, string? name, Attribute[] attributes)
            {
                _enum = @enum;
                _value = value;
                _name = name;
                _attributes = attributes;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(TEnum @enum) => Enums<TEnum>.Equals(@enum, _enum);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ulong value) => _value == value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ReadOnlySpan<char> name) => name.Equals(_name, StringComparison.OrdinalIgnoreCase);
        }

        internal static readonly int Count;
        internal static readonly TEnum[] _members;
        internal static readonly EnumData[] _memberData;

        public static IReadOnlyList<Attribute> Attributes { get; }
        public static bool HasFlags { get; }
        
        public static IEnumerable<string> Names => _memberData.Select(md => md.Name);
        public static IReadOnlyList<TEnum> Members => _members;
        
        static Enums()
        {
            var enumType = typeof(TEnum);
            Debug.Assert(enumType.IsEnum);
            var enumAttributes = Attribute.GetCustomAttributes(enumType);
            Attributes = enumAttributes;
            HasFlags = enumAttributes.Any(attr => attr is FlagsAttribute);
            var memberFields = enumType.GetFields(Reflect.StaticFlags);
            Count = memberFields.Length;
            _members = new TEnum[Count];
            _memberData = new EnumData[Count];
            for (var i = 0; i < Count; i++)
            {
                var field = memberFields[i];
                TEnum e = (TEnum) field.GetValue(null)!;
                _members[i] = e;
                var enumData = new EnumData(e, GetValue(e), field.Name, Attribute.GetCustomAttributes(field));
                _memberData[i] = enumData;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong GetValue(TEnum @enum)
        {
            Ldarg(nameof(@enum));
            return Return<ulong>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TEnum GetMember(ulong value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }

        public static bool IsDefined(TEnum @enum)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_memberData[i].Equals(@enum))
                    return true;
            }
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(TEnum x, TEnum y)
        {
            Ldarg(nameof(x));
            Ldarg(nameof(y));
            Ceq();
            return Return<bool>();
        }
        
        #region Parse
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(bool value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(char value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(byte value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(sbyte value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(short value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(ushort value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(int value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(uint value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(long value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(ulong value)
        {
            Ldarg(nameof(value));
            return Return<TEnum>();
        }

        public static bool TryParse(ulong value, out TEnum @enum)
        {
            foreach (var data in _memberData)
            {
                if (data.Equals(value))
                {
                    @enum = data.Enum;
                    return true;
                }
            }

            @enum = default;
            return false;
        }
        
        public static bool TryParse(ReadOnlySpan<char> text, out TEnum @enum)
        {
            if (ulong.TryParse(text, out ulong value))
            {
                foreach (var data in _memberData)
                {
                    if (data.Equals(value) || data.Equals(text))
                    {
                        @enum = data.Enum;
                        return true;
                    }
                }
            }
            else
            {
                foreach (var data in _memberData)
                {
                    if (data.Equals(text))
                    {
                        @enum = data.Enum;
                        return true;
                    }
                }
            }

            if (Enum.TryParse(typeof(TEnum), text.ToString(), true, out object? obj))
            {
                @enum = (TEnum)obj!;
                return true;
            }

            @enum = default;
            return false;
        }
        
        #endregion

        public static string? GetName(TEnum @enum)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_memberData[i].Equals(@enum))
                    return _memberData[i].Name;
            }
            return null;
        }
        
        #region Flags

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum And(TEnum x, TEnum y)
        {
            Ldarg(nameof(x));
            Ldarg(nameof(y));
            Emit.And();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Or(TEnum x, TEnum y)
        {
            Ldarg(nameof(x));
            Ldarg(nameof(y));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Xor(TEnum x, TEnum y)
        {
            Ldarg(nameof(x));
            Ldarg(nameof(y));
            Emit.Xor();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Not(TEnum @enum)
        {
            Ldarg(nameof(@enum));
            Emit.Not();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(TEnum @enum, TEnum flag)
        {
            // (e & flag) == flag
            Emit.Ldarg(nameof(flag));
            Emit.Dup();
            Emit.Ldarg(nameof(@enum));
            Emit.And();
            Emit.Ceq();
            return Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FlagCount(TEnum @enum)
        {
            Ldarg(nameof(@enum));
            Conv_U8();
            Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
            return Return<int>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlags(TEnum @enum, TEnum firstFlag)
        {
            return HasFlag(@enum, firstFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlags(TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        {
            return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlags(TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        {
            return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag) || HasFlag(@enum, thirdFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlags(TEnum @enum, params TEnum[] flags)
        {
            return flags.Any(flag => HasFlag(@enum, flag));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags(TEnum @enum, TEnum firstFlag)
        {
            return HasFlag(@enum, firstFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags(TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        {
            return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags(TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        {
            return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag) && HasFlag(@enum, thirdFlag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags(TEnum @enum, params TEnum[] flags)
        {
            return flags.All(flag => HasFlag(@enum, flag));
        }
      
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag(ref TEnum @enum, TEnum flag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(flag));
            Emit.Or();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(ref TEnum @enum, TEnum firstFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Or();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Or();
            Ldarg(nameof(secondFlag));
            Emit.Or();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Or();
            Ldarg(nameof(secondFlag));
            Emit.Or();
            Ldarg(nameof(thirdFlag));
            Emit.Or();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlags(ref TEnum @enum, params TEnum[] flags)
        {
            foreach (var flag in flags)
            {
                AddFlag(ref @enum, flag);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlag(ref TEnum @enum, TEnum flag)
        {
            // e &= ~flag
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(flag));
            Emit.Not();
            Emit.And();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(ref TEnum @enum, TEnum firstFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Not();
            Emit.And();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Not();
            Emit.And();
            Ldarg(nameof(secondFlag));
            Emit.Not();
            Emit.And();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        {
            Ldarg(nameof(@enum));
            Ldarg(nameof(@enum));
            Ldobj<TEnum>();
            Ldarg(nameof(firstFlag));
            Emit.Not();
            Emit.And();
            Ldarg(nameof(secondFlag));
            Emit.Not();
            Emit.And();
            Ldarg(nameof(thirdFlag));
            Emit.Not();
            Emit.And();
            Stobj<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFlags(ref TEnum @enum, params TEnum[] flags)
        {
            foreach (var flag in flags)
            {
                RemoveFlag(ref @enum, flag);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum first, TEnum second)
        {
            Ldarg(nameof(first));
            Ldarg(nameof(second));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum first, TEnum second, TEnum third)
        {
            Ldarg(nameof(first));
            Ldarg(nameof(second));
            Emit.Or();
            Ldarg(nameof(third));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum first, TEnum second, TEnum third, TEnum fourth)
        {
            Ldarg(nameof(first));
            Ldarg(nameof(second));
            Emit.Or();
            Ldarg(nameof(third));
            Emit.Or();
            Ldarg(nameof(fourth));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(params TEnum[] flags)
        {
            TEnum e = default;
            foreach (var flag in flags)
                AddFlag(ref e, flag);
            return e;
        }
        #endregion
    }
}