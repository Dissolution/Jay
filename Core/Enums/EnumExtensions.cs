using static InlineIL.IL;

namespace Jay.Enums;

public static class EnumExtensions
{
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
    public static bool Equal<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        Emit.Ldc_I4_0();
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
    public static bool LessThanOrEqual<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(right));
        Emit.Ldarg(nameof(left));
        Emit.Cgt();
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
    public static bool GreaterThanOrEqual<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(right));
        Emit.Ldarg(nameof(left));
        Emit.Clt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        if (LessThan(left, right)) return -1;
        if (GreaterThan(left, right)) return 1;
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I4();
        return Return<int>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }
}