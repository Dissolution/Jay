using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InlineIL.IL;

namespace Jay.Text;

public static class TextHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int memcmp(void* ptr1, void* ptr2, nuint byteCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(in char x, in char y, int charCount)
    {
        unsafe
        {
            return memcmp(Unsafe.AsPointer(in x),
                Unsafe.AsPointer(in y),
                (nuint)(charCount * 2));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(in char sourcePtr, ref char destPtr, int charCount)
    {
        Emit.Ldarg(nameof(destPtr));
        Emit.Ldarg(nameof(sourcePtr));
        Emit.Ldarg(nameof(charCount));
        Emit.Sizeof<char>();
        Emit.Mul();
        Emit.Cpblk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        var len = source.Length;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        CopyTo(in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? source, Span<char> dest)
    {
        var len = source?.Length ?? 0;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        CopyTo(in source!.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<char> source, char[] dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(char[] source, Span<char> dest)
    {
        CopyTo(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(char[] source, char[] dest)
    {
        CopyTo(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string source, Span<char> dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string source, char[] dest)
    {
        CopyTo(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, char[] y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, string? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, char[] y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, string? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[] y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y)
    {
        return string.Equals(x, y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, char[] y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, char[] y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[] x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[] y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y, StringComparison comparison)
    {
        return string.Equals(x, y, comparison);
    }
}