#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Jay.Text.Extensions;

/// <summary>
/// Internal extensions for compatability between .NET versions
/// </summary>
public static class CompatExtensions
{
#if NET48 || NETSTANDARD2_0
    public static bool Contains(
        this string str, 
        string value,
        StringComparison comparisonType)
    {
        return str.IndexOf(value, comparisonType) >= 0;
    }
#endif


#if NET48 || NETSTANDARD2_0 || NETSTANDARD2_1
    internal static ref readonly char GetPinnableReference(this string str)
    {
        unsafe
        {
            fixed (char* strPtr = str)
            {
                return ref Scary.PointerToRef<char>(strPtr);
            }
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref char GetPinnableReference(this char[] charArray)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference<char>(charArray);
#else
        return ref charArray[0];
#endif
    }
}