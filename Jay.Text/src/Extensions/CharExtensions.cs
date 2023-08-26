namespace Jay.Text.Extensions;

public static class CharExtensions
{
#if !NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(this char ch) => (uint)(ch - '0') <= '9' - '0';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterLower(this char ch) => ch is >= 'a' and <= 'z';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterUpper(this char ch) => ch is >= 'A' and <= 'Z';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(this char ch) => (ushort)ch < 128;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetter(this char ch) => (ch is >= 'a' and <= 'z') || (ch is >= 'A' and <= 'Z');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterOrDigit(this char ch) => IsAsciiLetter(ch) || IsAsciiDigit(ch);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(this char ch) => char.IsAsciiDigit(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterLower(this char ch) => char.IsAsciiLetterLower(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterUpper(this char ch) => char.IsAsciiLetterUpper(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(this char ch) => char.IsAscii(ch);

     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetter(this char ch) => char.IsAsciiLetter(ch);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterOrDigit(this char ch) => char.IsAsciiLetterOrDigit(ch);
#endif
}