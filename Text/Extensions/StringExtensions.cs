using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Text.Extensions;

public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsReadOnlySpan(this in char ch)
    {
        #if NETSTANDARD2_0_OR_GREATER
        unsafe
        {
            fixed (char* ptr = &ch)
            {
                return new ReadOnlySpan<char>(ptr, 1);
            }
        }
        #else
        return new ReadOnlySpan<char>(in ch);
#endif
    }
}


public static class StringExtensions
{
#if NETSTANDARD2_0_OR_GREATER
#endif
}

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

public static class CharSpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this Span<char> text)
    {
#if NETSTANDARD2_0_OR_GREATER
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this ReadOnlySpan<char> text)
    {
#if NETSTANDARD2_0_OR_GREATER
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }
}