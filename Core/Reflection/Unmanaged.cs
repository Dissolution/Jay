using System.Runtime.CompilerServices;
using static InlineIL.IL;

namespace Jay.Reflection;

public class Unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>()
        where T : unmanaged
    {
        Emit.Sizeof<T>();
        return Return<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T val1, T val2)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(val1));
        Emit.Ldarg(nameof(val2));
        Emit.Cgt();
        Emit.Brtrue("lblGT");
        Emit.Ldarg(nameof(val1));
        Emit.Ret();
        MarkLabel("lblGT");
        Emit.Ldarg(nameof(val2));
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T val1, T val2, T val3)
        where T : unmanaged
    {
        return Min(Min(val1, val2), val3);
    }
}