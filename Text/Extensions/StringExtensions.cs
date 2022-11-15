namespace Jay.Text.Extensions;

public static class StringExtensions
{
#if NETSTANDARD2_0_OR_GREATER
    public static unsafe ref char GetPinnableReference(this string str)
    {
        fixed (char* ptr = str)
        {
            return ref Unsafe.AsRef<char>(ptr);
        }
    }
#endif
}