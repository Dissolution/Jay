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
}