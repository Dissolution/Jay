using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay
{
    internal sealed class EnumInfo<TEnum>
        where TEnum : unmanaged, Enum
    {
        internal readonly FieldInfo _member;
        internal readonly ulong _value;
        
        public TEnum Enum { get; }
        public string Name { get; }
        public Attribute[] Attributes { get; }

        public EnumInfo(FieldInfo member)
        {
            _member = member;
            this.Enum = (TEnum) member.GetValue(null)!;
            _value = Enums<TEnum>.ULong(Enum);
            this.Name = member.Name;
            this.Attributes = Attribute.GetCustomAttributes(member, true);
        }
    }
    
    
    public static class Enums<TEnum>
        where TEnum : unmanaged, Enum
    {
        private static readonly EnumInfo<TEnum>[] _infos;

        static Enums()
        {
            var enumType = typeof(TEnum);
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            int len = fields.Length;
            _infos = new EnumInfo<TEnum>[len];
            for (var i = 0; i < len; i++)
            {
                _infos[i] = new EnumInfo<TEnum>(fields[i]);
            }
        }
        
        public static bool TryParse(ulong value, out TEnum @enum)
        {
            foreach (var info in _infos)
            {
                if (info._value == value)
                {
                    @enum = info.Enum;
                    return true;
                }
            }

            @enum = default;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> text, out TEnum @enum)
        {
            foreach (var info in _infos)
            {
                if (MemoryExtensions.Equals(info.Name, text, StringComparison.OrdinalIgnoreCase))
                {
                    @enum = info.Enum;
                    return true;
                }
            }

            @enum = default;
            return false;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ULong(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            // This may not be required since an enum can only have a byte, sbyte, short, ushort, int, uint, long, ulong backing
            Emit.Conv_U8();
            return Return<ulong>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FlagCount(TEnum @enum)
        {
            return BitOperations.PopCount(ULong(@enum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum flag1, TEnum flag2)
        {
            Emit.Ldarg(nameof(flag1));
            Emit.Ldarg(nameof(flag2));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum flag1, TEnum flag2, TEnum flag3)
        {
            Emit.Ldarg(nameof(flag1));
            Emit.Ldarg(nameof(flag2));
            Emit.Or();
            Emit.Ldarg(nameof(flag3));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(TEnum flag1, TEnum flag2, TEnum flag3, TEnum flag4)
        {
            Emit.Ldarg(nameof(flag1));
            Emit.Ldarg(nameof(flag2));
            Emit.Or();
            Emit.Ldarg(nameof(flag3));
            Emit.Or();
            Emit.Ldarg(nameof(flag4));
            Emit.Or();
            return Return<TEnum>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Combine(params TEnum[] flags)
        {
            TEnum @enum = default;
            foreach (var flag in flags)
            {
                @enum = Or(@enum, flag);
            }
            return @enum;
        }

        /// <summary>
        /// Returns the bitwise AND (&amp;) of two <typeparamref name="TEnum"/> values
        /// </summary>
        /// <param name="first">The first <see langword="enum"/> to AND.</param>
        /// <param name="second">The second <see langword="enum"/> to AND.</param>
        /// <returns>(<paramref name="first"/> &amp; <paramref name="second"/>)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum And(TEnum first, TEnum second)
        {
            Emit.Ldarg(nameof(first));
            Emit.Ldarg(nameof(second));
            Emit.And();
            return Return<TEnum>();
        }
        
        /// <summary>
        /// Returns the bitwise OR (|) of two <typeparamref name="TEnum"/> values
        /// </summary>
        /// <param name="first">The first <see langword="enum"/> to OR.</param>
        /// <param name="second">The second <see langword="enum"/> to OR.</param>
        /// <returns>(<paramref name="first"/> | <paramref name="second"/>)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Or(TEnum first, TEnum second)
        {
            Emit.Ldarg(nameof(first));
            Emit.Ldarg(nameof(second));
            Emit.Or();
            return Return<TEnum>();
        }
        
        /// <summary>
        /// Returns the bitwise XOR (^) of two <typeparamref name="TEnum"/> values
        /// </summary>
        /// <param name="first">The first <see langword="enum"/> to XOR.</param>
        /// <param name="second">The second <see langword="enum"/> to XOR.</param>
        /// <returns>(<paramref name="first"/> ^ <paramref name="second"/>)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Xor(TEnum first, TEnum second)
        {
            Emit.Ldarg(nameof(first));
            Emit.Ldarg(nameof(second));
            Emit.Xor();
            return Return<TEnum>();
        }
        
        /// <summary>
        /// Returns the bitwise complement (~) of an <typeparamref name="TEnum"/> value
        /// </summary>
        /// <param name="enum">The <see langword="enum"/> to complement.</param>
        /// <returns>(~<paramref name="enum"/>)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Not(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            Emit.Not();
            return Return<TEnum>();
        }
        
        /// <summary>
        /// Returns the negation (!) of an <typeparamref name="TEnum"/> value
        /// </summary>
        /// <param name="enum">The <see langword="enum"/> to negate.</param>
        /// <returns>(!<paramref name="enum"/>)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Neg(TEnum @enum)
        {
            Emit.Ldarg(nameof(@enum));
            Emit.Neg();
            return Return<TEnum>();
        }
    }
}