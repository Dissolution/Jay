using static InlineIL.IL;

namespace Jay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>enum</c>
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Is this <paramref name="enum"/> == <c>default(</c><typeparamref name="TEnum"/><c>)</c>?
    /// </summary>
    /// <typeparam name="TEnum">
    /// The <see cref="Type"/> of this <c>enum</c>
    /// </typeparam>
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
    public static bool Equal<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Clt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(other));
        Emit.Ldarg(nameof(@enum));
        Emit.Cgt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Cgt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(other));
        Emit.Ldarg(nameof(@enum));
        Emit.Clt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        if (LessThan(@enum, other)) return -1;
        if (GreaterThan(@enum, other)) return 1;
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
    public static long ToInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I8();
        return Return<long>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }

    public static TEnum FromUInt64<TEnum>(ulong value)
    {
        Emit.Ldarg(nameof(value));
        return Return<TEnum>();
    }
}