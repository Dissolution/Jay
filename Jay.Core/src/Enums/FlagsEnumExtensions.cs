// ReSharper disable EntityNameCapturedOnly.Global

using System.Diagnostics;
using InlineIL;
using Jay.Utilities;
using static InlineIL.IL;


namespace Jay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>[Flags] enum</c>
/// </summary>
/// <remarks>
/// There is no way to constrict these extensions to flagged enums,
/// so beware of odd behavior for non-flagged enums
/// </remarks>
public static class FlagsEnumExtensions
{
    /// <summary>
    /// Returns a bitwise NOT (<c>~</c>) of this <paramref name="enum"/>
    /// </summary>
    /// <remarks>
    /// Also known as the inverse or bitwise complement
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Not<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Not();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise AND (<c>&amp;</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being AND'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to AND</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to AND</param>
    /// <returns>The two <see langword="enum"/>s AND'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> &amp; <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum And<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.And();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise OR (<c>|</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being OR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to OR</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to OR</param>
    /// <returns>The two <see langword="enum"/>s OR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> | <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Or<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise XOR (<c>^</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being XOR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to XOR</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to XOR</param>
    /// <returns>The two <see langword="enum"/>s XOR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> ^ <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Xor<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftLeft<TEnum>(this TEnum @enum, int count)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shl();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftRight<TEnum>(this TEnum @enum, int count)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shr_Un(); // 99%+ of enums are unsigned
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagCount<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(MathHelper), nameof(MathHelper.PopCount), typeof(ulong)));
        return Return<int>();
    }

    public static TEnum[] GetFlags<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        int flagCount = FlagCount(@enum);
        var flags = new TEnum[flagCount];
        int f = 0;
        int maxBits = Scary.SizeOf<TEnum>() * 8;
        ulong enumValue = EnumExtensions.ToUInt64(@enum);
        //string enumValueBits = Convert.ToString((long)enumValue, 2);
        for (var shift = 0; shift < maxBits; shift++)
        {
            ulong mask = 1UL << shift;
            //string maskBits = Convert.ToString((long)mask, 2);
            if ((enumValue & mask) != 0UL)
            {
                var flag = EnumExtensions.FromUInt64<TEnum>(mask);
                flags[f++] = flag;
            }
        }
        Debug.Assert(f == flagCount);
        return flags;
    }

    /// <summary>
    /// Adds the <paramref name="flag"/> to this <paramref name="enum"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        @enum = Or(@enum, flag);
    }

    /// <summary>
    /// Returns this <paramref name="enum"/> combined with the <paramref name="flag"/>
    /// </summary>
    /// <remarks>
    /// <c>return</c> <paramref name="enum"/> <c>|</c> <paramref name="flag"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return Or(@enum, flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        @enum = And(@enum, Not(flag));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithoutFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return And(@enum, Not(flag));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2)
        where TEnum : struct, Enum
    {
        return @enum.And(flag1.Or(flag2))
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2, TEnum flag3)
        where TEnum : struct, Enum
    {
        return @enum.And(
                flag1.Or(flag2)
                    .Or(flag3))
            .NotEqual(default);
    }

    public static bool HasAnyFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum flag = default;
        for (var i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .Equal(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2)
        where TEnum : struct, Enum
    {
        TEnum flag = flag1.Or(flag2);
        return @enum.And(flag)
            .Equal(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2, TEnum flag3)
        where TEnum : struct, Enum
    {
        TEnum flag = flag1.Or(flag2)
            .Or(flag3);
        return @enum.And(flag)
            .Equal(flag);
    }

    public static bool HasAllFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum flag = default;
        for (var i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .Equal(flag);
    }
}