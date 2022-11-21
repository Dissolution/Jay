using System.Runtime.InteropServices;

namespace Jay.Text.Extensions;

public static class CharacterArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this char[] charArray, char ch)
    {
        return MemoryExtensions.Contains<char>(charArray, ch);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
        return ref MemoryMarshal.GetArrayDataReference<char>(charArray);
    }
}