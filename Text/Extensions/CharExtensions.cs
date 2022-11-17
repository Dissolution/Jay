namespace Jay.Text.Extensions;

public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this in char ch)
    {
        return new ReadOnlySpan<char>(in ch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiDigit(this char ch) => char.IsAsciiDigit(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterLower(this char ch) => char.IsAsciiLetterLower(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiLetterUpper(this char ch) => char.IsAsciiLetterUpper(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(this char ch) => char.IsAscii(ch);
}