using System.Numerics;

using InlineIL;

using Jay.Extensions;

using static InlineIL.IL;

namespace Jay.Reflection.Enums;

public static class EnumExtensions
{
    /// <summary>
    /// Is this <c>enum</c> the default <typeparamref name="TEnum"/> value?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }

    #region Bitwise Operations
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Not<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Not();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Neg<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Neg();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Or<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum And<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.And();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Xor<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Xor();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Clt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessOrEqual<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Cgt();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Cgt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterOrEqual<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Clt();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftedLeft<TEnum>(this TEnum @enum, int count)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shl();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ShiftedRight<TEnum>(this TEnum @enum, int count)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(count));
        Emit.Shr_Un();
        return Return<TEnum>();
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagCount<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
        return Return<int>();
    }

    /// <summary>
    /// Does this <paramref name="@enum"/> have the given <paramref name="flag"/> set?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
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
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag) || HasFlag(@enum, thirdFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        return flags.Any(flag => HasFlag(@enum, flag));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag) && HasFlag(@enum, thirdFlag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        return flags.All(flag => HasFlag(@enum, flag));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        Emit.Ldarg(nameof(thirdFlag));
        Emit.Or();
        return Return<TEnum>();
    }

    public static TEnum WithFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum e = @enum;
        for (var i = 0; i < flags.Length; i++)
        {
            e = e.Or(flags[i]);
        }
        return e;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Or();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Or();
        Emit.Ldarg(nameof(thirdFlag));
        Emit.Or();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlags<TEnum>(this ref TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        for (var i = 0; i < flags.Length; i++)
        {
            @enum.AddFlag(flags[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        // e &= ~flag
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(flag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldobj<TEnum>();
        Emit.Ldarg(nameof(firstFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(secondFlag));
        Emit.Not();
        Emit.And();
        Emit.Ldarg(nameof(thirdFlag));
        Emit.Not();
        Emit.And();
        Emit.Stobj<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlags<TEnum>(this ref TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        foreach (var flag in flags)
        {
            @enum.RemoveFlag(flag);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        Emit.Brfalse("notEqual");
        Emit.Ldc_I4(0);
        Emit.Ret();
        MarkLabel("notEqual");
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Clt();
        Emit.Brfalse("greaterThan");
        Emit.Ldc_I4(-1);
        Emit.Ret();
        MarkLabel("greaterThan");
        Emit.Ldc_I4(1);
        Emit.Ret();
        throw Unreachable();
    }
}