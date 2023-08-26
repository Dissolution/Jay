using static InlineIL.IL;

namespace Jay.CodeGen.Enums;

public static class EnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        Emit.Ldc_I4_0();
        Emit.Cgt();
        return Return<bool>();
    }
}