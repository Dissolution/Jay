namespace Jay.Reflection.Enums;

public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum>, IEqualityComparer,
                                          IComparer<TEnum>, IComparer
    where TEnum : struct, Enum
{
    internal EnumComparer()
    {
        // We only want to be constructed by EnumTypeInfo<TEnum> and cached there!
    }

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
            // null sorts first
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
            return y is not TEnum;
        }

        if (y is not TEnum yEnum)
        {
            return false;
        }

        return Equals(xEnum, yEnum);
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
        // Load the enum (of unknown size)
        Emit.Ldarg(nameof(@enum));
        // Convert it to uint64
        Emit.Conv_U8();
        // Load the int32 value of 32
        Emit.Ldc_I4(32);
        // Shift the uint64 enum value right those 32 places (we want the highest 32 bits)
        Emit.Shr();
        // Load the enum again
        Emit.Ldarg(nameof(@enum));
        // Convert it to a uint32 (which will truncate if the base type is bigger)
        Emit.Conv_U4();
        // XOR these uint32 (lowest bits) with the previous ones (highest bits)
        Emit.Xor();
        // return the int32 on the stack
        return Return<int>();
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is not TEnum @enum) return 0;
        return GetHashCode(@enum);
    }
}