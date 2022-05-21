using static InlineIL.IL;

namespace Jay.Dumping.Refactor;

public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum>, IEqualityComparer,
                                          IComparer<TEnum>, IComparer
    where TEnum : struct, Enum
{
    public static EnumComparer<TEnum> Default { get; } = new EnumComparer<TEnum>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(TEnum x, TEnum y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Bge("x_ge_y");
        Emit.Ldc_I4_M1();
        Emit.Ret();
        MarkLabel("x_ge_y");
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ble("x_le_y");
        Emit.Ldc_I4_1();
        Emit.Ret();
        MarkLabel("x_le_y");
        Emit.Ldc_I4_0();
        Emit.Ret();
        throw Unreachable();
    }

    int IComparer.Compare(object? x, object? y)
    {
        if (x is null || x is not TEnum xEnum)
        {
            if (y is null || y is not TEnum) 
                return 0;
            return -1;
        }

        if (y is null || y is not TEnum yEnum)
        {
            return 1;
        }

        return Compare(xEnum, yEnum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(TEnum x, TEnum y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ceq();
        return Return<bool>();
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x is null || x is not TEnum xEnum)
        {
            if (y is null || y is not TEnum)
                return true;
            return false;
        }

        if (y is null || y is not TEnum yEnum)
        {
            return false;
        }

        return Equals(xEnum, yEnum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(TEnum e)
    {
        Emit.Ldarg(nameof(e));
        Emit.Conv_I8();
        Emit.Ldc_I4(32);
        Emit.Shr();
        Emit.Ldarg(nameof(e));
        Emit.Conv_I4();
        Emit.Xor();
        return Return<int>();
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        if (obj is TEnum e) return GetHashCode(e);
        return obj.GetHashCode();
    }
}