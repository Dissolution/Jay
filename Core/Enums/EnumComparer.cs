using static InlineIL.IL;

namespace Jay.Enums;

public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum>, IEqualityComparer,
                                          IComparer<TEnum>, IComparer
    where TEnum : struct, Enum
{
    /// <summary>
    /// Gets the default <see cref="EnumComparer{TEnum}"/>
    /// </summary>
    public static EnumComparer<TEnum> Default { get; } = new EnumComparer<TEnum>();

    /// <summary>
    /// Compares two <typeparamref name="TEnum"/> values
    /// </summary>
    /// <param name="x">The first <typeparamref name="TEnum"/> to compare</param>
    /// <param name="y">The second <typeparamref name="TEnum"/> to compare</param>
    /// <returns>
    /// -1: x is less than y
    /// 0: x is equal to y
    /// 1: x is greater than y 
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(TEnum x, TEnum y)
    {
        // x >= y ? goto x_ge_y : return -1
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Bge("x_ge_y");
        Emit.Ldc_I4_M1();
        Emit.Ret();
        
        // x <= y ? goto x_le_y : return 1;
        MarkLabel("x_ge_y");
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ble("x_le_y");
        Emit.Ldc_I4_1();
        Emit.Ret();
        
        // x == y
        // return 0;
        MarkLabel("x_le_y");
        Emit.Ldc_I4_0();
        Emit.Ret();
        throw Unreachable();
    }

    int IComparer.Compare(object? x, object? y)
    {
        // null and non-TEnum are treated the same
        if (x is not TEnum xEnum)
        {
            if (y is not TEnum) return 0;
            // null always sorts first
            return -1;
        }

        if (y is not TEnum yEnum)
        {
            return 1;
        }

        return Compare(xEnum, yEnum);
    }

    /// <summary>
    /// Determines if two <typeparamref name="TEnum"/> values are equal
    /// </summary>
    /// <param name="x">The first enum to compare</param>
    /// <param name="y">The second enum to compare</param>
    /// <returns>True if x == y; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(TEnum x, TEnum y)
    {
        // return x == y;
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ceq();
        return Return<bool>();
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        // All non-TEnums are the same
        if (x is not TEnum xEnum)
        {
            if (y is not TEnum) return true;
            return false;
        }

        if (y is not TEnum yEnum)
        {
            return false;
        }

        return Equals(xEnum, yEnum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AsULong(TEnum @enum)
    {
        Emit.Ldarg_0();
        Emit.Conv_I8();
        return Return<ulong>();
    }
    
    /// <summary>
    /// Gets a hashcode for an <paramref name="enum"/>
    /// </summary>
    /// <remarks>
    /// <see cref="Enum"/>.<see cref="M:Enum.GetHashCode"/> is fine and has its own implementation that is probably better.
    /// But I wanted the challenge of doing it very simply in IL with no knowledge of the base type.
    /// Though long/ulong backed enums are rare, I wanted to support every case
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(TEnum @enum)
    {
        // Load the enum (it is a value type)
        Emit.Ldarg(nameof(@enum));
        // Convert it to a int64 
        Emit.Conv_I8();
        // Load the int32 value of 32
        Emit.Ldc_I4(32);
        // Shift the int64 enum value right those 32 places (we want the highest 32 bits)
        Emit.Shr();
        // Load the enum again
        Emit.Ldarg(nameof(@enum));
        // Convert it to a int32 (which will truncate if the base type is bigger)
        Emit.Conv_I4();
        // XOR this int32 (lowest bits) with the previous ones (highest bits)
        Emit.Xor();
        // return the int32 on the stack
        return Return<int>();
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        if (obj is TEnum e) return GetHashCode(e);
        return obj.GetHashCode();
    }
}