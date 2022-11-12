using System.Globalization;
using System.Runtime.InteropServices;
using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay.Text;

public static class TextHelper
{
    internal const string DIGITS = "0123456789";
    internal const string UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    internal const string LOWERCASE = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// The offset between an uppercase ascii letter and its lowercase equivalent
    /// </summary>
    internal const int UPPERCASE_OFFSET = 'a' - 'A';
    
    public static ReadOnlySpan<char> Digits => DIGITS;
    public static ReadOnlySpan<char> Uppercase => UPPERCASE;
    public static ReadOnlySpan<char> Lowercase => LOWERCASE;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(char c) => c is <= '9' and >= '0';
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLower(char c) => c is <= 'z' and >= 'a';
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiUpper(char c) => c is <= 'Z' and >= 'A';
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(char c) => c <= 127 && c >= 0;
    
    /// <summary>
    /// Transforms the specified characters into Uppercase, using char.ToUpper(c)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUppercase(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        for (var i = text.Length - 1; i >= 0; i--)
        {
            buffer[i] = char.ToUpper(text[i]);
        }
        return new string(buffer);
    }

    /// <summary>
    /// Transforms the specified characters into Uppercase, using char.ToLower(c)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowercase(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        for (var i = text.Length - 1; i >= 0; i--)
        {
            buffer[i] = char.ToLower(text[i]);
        }
        return new string(buffer);
    }
    
    public static string ToTitleCase(ReadOnlySpan<char> text)
    {
        var str = new string(text);
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
    
    [return: NotNullIfNotNull("text")]
    public static string? ToTitleCase(string? text)
    {
        if (text is null) return null;
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
    }

    /// <summary>
    /// Reverses the order of the specified characters.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToReverse(ReadOnlySpan<char> text)
    {
        int end = text.Length - 1;
        Span<char> buffer = stackalloc char[text.Length];
        for (var i = 0; i <= end; i++)
        {
            buffer[i] = text[end - i];
        }
        return new string(buffer);
    }

    public static string Refine(string? text) => Refine(text.AsSpan());

    public static string Refine(params char[]? chars) => Refine(chars.AsSpan());

    public static string Refine(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        int b = 0;
        char ch;
        for (var i = 0; i < text.Length; i++)
        {
            ch = text[i];
            if (ch is >= '0' and <= '9' || ch is >= 'A' and <= 'Z')
            {
                buffer[b++] = ch;
            }
            else if (ch is >= 'a' and <= 'z')
            {
                buffer[b++] = (char)(ch - UPPERCASE_OFFSET);
            }
        }
        return new string(buffer.Slice(0, b));
    }
}