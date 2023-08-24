using static InlineIL.IL;

namespace Jay.Utilities;

/// <summary>
/// Similar to <see cref="System.Runtime.CompilerServices.Unsafe"/>, this helper class is full
/// of bad things you shouldn't use unless you understand what you are doing.
/// </summary>
public static class Scary
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref T source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>()
        where T : struct
    {
        Emit.Sizeof<T>();
        return Return<int>();
    }
    

}