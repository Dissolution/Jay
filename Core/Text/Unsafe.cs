using static InlineIL.IL;

namespace Jay.Text;

internal static unsafe class Unsafe
{
    public static char* AsPointer(in char ch)
    {
        Emit.Ldarg(nameof(ch));
        return ReturnPointer<char>();
    }
}