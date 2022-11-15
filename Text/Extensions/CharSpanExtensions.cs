namespace Jay.Text.Extensions;

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