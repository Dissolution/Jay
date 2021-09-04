using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay
{
    public static class Enums
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum And<TEnum>(this TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => Enums<TEnum>.And(@enum, flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FlagCount<TEnum>(this TEnum @enum)
            where TEnum : unmanaged, Enum
            => Enums<TEnum>.FlagCount(@enum);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
            where TEnum : unmanaged, Enum
            => @enum = Enums<TEnum>.Combine(@enum, flag);
    }

    public static class Enums<TEnum>
        where TEnum : unmanaged, Enum
    {
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