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
}