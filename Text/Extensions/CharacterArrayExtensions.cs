using System.Runtime.InteropServices;

namespace Jay.Text.Extensions;

public static class CharacterArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this T[] array, T item)
        where T : IEquatable<T>
    {
#if NETSTANDARD2_0_OR_GREATER
        return array.AsSpan().IndexOf<T>(item) >= 0;
#else
        return MemoryExtensions.Contains<T>(array, item);
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
#if NETSTANDARD2_0_OR_GREATER
        return ref charArray[0];
#else
        return ref MemoryMarshal.GetArrayDataReference<char>(charArray);
#endif
    }
}