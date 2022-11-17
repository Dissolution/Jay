using System.Runtime.InteropServices;

namespace Jay.Text.Extensions;

public static class CharacterArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this T[] array, T item)
        where T : IEquatable<T>
    {
        return MemoryExtensions.Contains<T>(array, item);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
        return ref MemoryMarshal.GetArrayDataReference<char>(charArray);
    }
}