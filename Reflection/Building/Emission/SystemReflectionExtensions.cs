using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Jay.Reflection.Emission;

internal static class SystemReflectionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this Label label) => label.GetHashCode() <= sbyte.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this LocalBuilder local)
    {
        // if (local is null)
        //     throw new ArgumentNullException(nameof(local));
        // var index = local.LocalIndex;
        // if (index < 0 || index > 65534)
        //     throw new ArgumentOutOfRangeException(nameof(local), index, "LocalBuilder index must be between 0 and 65534");
        return local.LocalIndex <= byte.MaxValue;
    }

    public static OpCode GetCallOpCode(this MethodBase method)
    {
        //If the method is static, we know it can never be null, so we can Call
        if (method.IsStatic)
            return OpCodes.Call;
        //If the method owner is a struct, it can also never be null, so we can Call
        var source = method.ReflectedType ?? method.DeclaringType;
        if (source != null && source.IsValueType)
            return OpCodes.Call;
        return OpCodes.Callvirt;
    }


}