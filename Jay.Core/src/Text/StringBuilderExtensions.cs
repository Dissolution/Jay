
using System.Text;

namespace Jay.Text;

public static class StringBuilderExtensions
{
#if NET48 || NETSTANDARD2_0
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

    public static StringBuilder Append<T>(
        this StringBuilder builder,
        T? value,
        string? format = null,
        IFormatProvider? provider = null)
    {
        string? str;
        if (value is null)
        {
            return builder;
        }
        // No boxing for value types
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value.ToString();
        }
        return builder.Append(str);
    }
}
