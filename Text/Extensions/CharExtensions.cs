namespace Jay.Text.Extensions;

public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this in char ch)
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