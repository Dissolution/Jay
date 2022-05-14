using System.Runtime.InteropServices;
using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay.Text;

public static class TextHelper
{
    private const string _digits = "0123456789";
    private const string _upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string _lowerCase = "abcdefghijklmnopqrstuvwxyz";

    public static ReadOnlySpan<char> Digits => _digits;
    public static ReadOnlySpan<char> UpperCase => _upperCase;
    public static ReadOnlySpan<char> LowerCase => _lowerCase;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void CopyTo(char* sourcePtr, ref char destPtr, int charCount)
    {
        Emit.Ldarg(nameof(destPtr));
        Emit.Ldarg(nameof(sourcePtr));
        Emit.Ldarg(nameof(charCount));
        Emit.Sizeof<char>();
        Emit.Mul();
        Emit.Cpblk();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Copy(in char sourcePtr, ref char destPtr, int charCount)
    {
        Emit.Ldarg(nameof(destPtr));
        Emit.Ldarg(nameof(sourcePtr));
        Emit.Ldarg(nameof(charCount));
        Emit.Sizeof<char>();
        Emit.Mul();
        Emit.Cpblk();
    }

    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        var len = source.Length;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        Copy(in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }

    public static bool TryCopyTo(string? source, Span<char> dest)
    {
        var len = source?.Length ?? 0;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        Copy(in source!.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        Copy(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(ReadOnlySpan<char> source, char[] dest)
    {
        Copy(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(char[] source, Span<char> dest)
    {
        Copy(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(char[] source, char[] dest)
    {
        Copy(in MemoryMarshal.GetArrayDataReference<char>(source),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(string source, Span<char> dest)
    {
        Copy(in source.GetPinnableReference(),
               ref dest.GetPinnableReference(),
               source.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CopyTo(string source, char[] dest)
    {
        Copy(in source.GetPinnableReference(),
               ref MemoryMarshal.GetArrayDataReference<char>(dest),
               source.Length);
    }

    #region Equals
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, string? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, string? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y)
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
    public static bool Equals(ReadOnlySpan<char> x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y, StringComparison comparison)
    {
        return string.Equals(x, y, comparison);
    }
    #endregion

}