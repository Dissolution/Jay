using System.Runtime.CompilerServices;
using InlineIL;

namespace Jay;

public static class GenericExtensions
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<T>(this T? value)
    {
        IL.Emit.Ldarg(nameof(value));
        IL.Emit.Brfalse("yup");
        IL.Emit.Ldc_I4_0();
        IL.Emit.Ret();
        IL.MarkLabel("yup");
        IL.Emit.Ldc_I4_1();
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

    public static ReadOnlySpan<char> ToReadOnlyText<T>(this T? value)
    {
        if (value is null) return default;
        if (value is string str) return str.AsSpan();
        if (value is char[] chars) return chars.AsSpan();
        if (value is char ch) return ch.AsReadOnlySpan();
        return value.ToString().AsSpan();
    }
}