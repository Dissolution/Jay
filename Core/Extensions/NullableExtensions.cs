using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Extensions;

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