using System.Runtime.CompilerServices;

namespace Jay;

public static class NullableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(this T? nullable, out T value)
        where T : struct
    {
        if (nullable.HasValue)
        {
            value = nullable.Value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
}