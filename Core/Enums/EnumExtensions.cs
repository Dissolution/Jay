using System.Numerics;
using InlineIL;
using static InlineIL.IL;

namespace Jay.Enums;

public static class EnumExtensions
{
    public static EnumMemberInfo<TEnum> GetInfo<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        return EnumInfo.For<TEnum>(@enum);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
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
    public static int FlagCount<TEnum>(this TEnum @enum)
        where TEnum : unmanaged, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
        return Return<int>();
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) || HasFlag(@enum, secondFlag) || HasFlag(@enum, thirdFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        return flags.Any(flag => HasFlag(@enum, flag));
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum firstFlag, TEnum secondFlag, TEnum thirdFlag)
        where TEnum : unmanaged, Enum
    {
        return HasFlag(@enum, firstFlag) && HasFlag(@enum, secondFlag) && HasFlag(@enum, thirdFlag);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : unmanaged, Enum
    {
        return flags.All(flag => HasFlag(@enum, flag));
    }
      
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            AddFlag(ref @enum, flag);
        }
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
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
        where TEnum : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            RemoveFlag(ref @enum, flag);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : unmanaged, Enum
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