using System.Globalization;

// ReSharper disable InvokeAsExtensionMethod
// ^ I want to be sure I'm calling the very specific version of a method


// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay.Text;

public static class TextHelper
{
    public const string Digits = "0123456789";
    public const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// The offset between an uppercase ascii letter and its lowercase equivalent
    /// </summary>
    internal const int UppercaseOffset = 'a' - 'A';

    public static class Unsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void CopyBlock(char* sourcePtr, ref char destPtr, int charCount)
        {
            Emit.Ldarg(nameof(destPtr));
            Emit.Ldarg(nameof(sourcePtr));
            Emit.Ldarg(nameof(charCount));
            Emit.Sizeof<char>();
            Emit.Mul();
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyBlock(in char sourcePtr, ref char destPtr, int charCount)
        {
            Emit.Ldarg(nameof(destPtr));
            Emit.Ldarg(nameof(sourcePtr));
            Emit.Ldarg(nameof(charCount));
            Emit.Sizeof<char>();
            Emit.Mul();
            Emit.Cpblk();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(ReadOnlySpan<char> source, Span<char> dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(ReadOnlySpan<char> source, char[] dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(char[] source, Span<char> dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(char[] source, char[] dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
#if NETSTANDARD2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void CopyTo(string source, Span<char> dest)
        {
            fixed (char* sourcePtr = source)
            {
                CopyBlock(sourcePtr,
                    ref dest.GetPinnableReference(),
                    source.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void CopyTo(string source, char[] dest)
        {
            fixed (char* sourcePtr = source)
            {
                CopyBlock(sourcePtr,
                    ref dest.GetPinnableReference(),
                    source.Length);
            }
        }
#else
          [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(string source, Span<char> dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyTo(string source, char[] dest)
        {
            CopyBlock(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
#endif
    }


    public static bool TryCopyTo(ReadOnlySpan<char> source, Span<char> dest)
    {
        var len = source.Length;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        Unsafe.CopyBlock(in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }

#if NETSTANDARD2_0_OR_GREATER
    public static unsafe bool TryCopyTo(string? source, Span<char> dest)
    {
        var len = source?.Length ?? 0;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        {
            fixed (char* sourcePtr = source!)
            {
                CopyBlock(sourcePtr, ref dest.GetPinnableReference(), len);
            }
        }
        return true;
    }
#else
    public static bool TryCopyTo(string? source, Span<char> dest)
    {
        var len = source?.Length ?? 0;
        if (len == 0) return true;
        if (len > dest.Length) return false;
        Unsafe.CopyBlock(in source!.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            len);
        return true;
    }
#endif


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
        return MemoryExtensions.SequenceEqual<char>(x, y.AsSpan());
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
        return MemoryExtensions.SequenceEqual<char>(x, y.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y)
    {
        return MemoryExtensions.SequenceEqual<char>(x.AsSpan(), y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x.AsSpan(), y);
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
        return MemoryExtensions.Equals(x, y.AsSpan(), comparison);
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
        return MemoryExtensions.Equals(x, y.AsSpan(), comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, ReadOnlySpan<char> y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x.AsSpan(), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x.AsSpan(), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y, StringComparison comparison)
    {
        return string.Equals(x, y, comparison);
    }

    #endregion

#if NETSTANDARD2_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(char c) => c is <= '9' and >= '0';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLower(char c) => c is <= 'z' and >= 'a';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiUpper(char c) => c is <= 'Z' and >= 'A';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(char c) => c <= 127;
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(char ch) => char.IsAsciiDigit(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterLower(char ch) => char.IsAsciiLetterLower(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterUpper(char ch) => char.IsAsciiLetterUpper(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(char ch) => char.IsAscii(ch);
#endif


    /// <summary>
    /// Transforms the specified characters into Uppercase, using char.ToUpper(c)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUppercaseString(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        for (var i = text.Length - 1; i >= 0; i--)
        {
            buffer[i] = char.ToUpper(text[i]);
        }


        return buffer.AsString();
    }

    /// <summary>
    /// Transforms the specified characters into Uppercase, using char.ToLower(c)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowercaseString(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        for (var i = text.Length - 1; i >= 0; i--)
        {
            buffer[i] = char.ToLower(text[i]);
        }

        return buffer.AsString();
    }

    public static string ToTitleCase(ReadOnlySpan<char> text)
    {
        var str = text.AsString();
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

        return buffer.AsString();
    }

    public static string Refine(string? text) => Refine(text.AsSpan());

    public static string Refine(params char[]? chars) => Refine(chars.AsSpan());

#if NETSTANDARD2_0_OR_GREATER
    public static string Refine(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        int b = 0;
        char ch;
        for (var i = 0; i < text.Length; i++)
        {
            ch = text[i];
            if ((ch >= 0 && ch <= 9) || (ch >= 'A' && ch <= 'Z'))
            {
                buffer[b++] = ch;
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                buffer[b++] = (char)(ch - UppercaseOffset);
            }
        }
        return buffer.Slice(0, b).AsString();
    }
#else
      public static string Refine(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        int b = 0;
        char ch;
        for (var i = 0; i < text.Length; i++)
        {
            ch = text[i];
            if (char.IsAsciiDigit(ch) || char.IsAsciiLetterUpper(ch))
            {
                buffer[b++] = ch;
            }
            else if (char.IsAsciiLetterLower(ch))
            {
                buffer[b++] = (char)(ch - UppercaseOffset);
            }
        }

        return new string(buffer.Slice(0, b));
    }
#endif


}