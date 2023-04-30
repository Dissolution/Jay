namespace Jay.Text;

public static class StringBuilderExtensions
{
    #if NETSTANDARD2_0
    public static StringBuilder Append(this StringBuilder builder, ReadOnlySpan<char> text)
    {
        unsafe
        {
            fixed (char* ptr = &text.GetPinnableReference())
            {
                builder.Append(ptr, text.Length);
            }
        }
        return builder;
    }
    #endif
}