using System.Globalization;

namespace Jay.Extensions;

public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    public static ReadOnlySpan<char> AsSpan(in this char ch)
    {
#if NET7_0 //_OR_GREATER
        return new ReadOnlySpan<char>(in ch);
#else
        unsafe
        {
            fixed (char* ptr = &ch)
            {
                return new ReadOnlySpan<char>((void*)ptr, 1);
            }
        }
#endif
    }

    /// <summary>
    /// Is this <see cref="char"/> a digit?
    /// </summary>
    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }

    /// <summary>
    /// Is this <see cref="char"/> considered white space?
    /// </summary>
    public static bool IsWhiteSpace(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    /// <summary>
    /// Converts this <see cref="char"/> into its UpperCase equivalent.
    /// </summary>
    public static char ToUpper(this char c)
    {
        return char.ToUpper(c);
    }

    /// <summary>
    /// Converts this <see cref="char"/> into its UpperCase equivalent.
    /// </summary>
    public static char ToUpper(this char c, CultureInfo culture)
    {
        return char.ToUpper(c, culture);
    }

    /// <summary>
    /// Converts this <see cref="char"/> into its LowerCase equivalent.
    /// </summary>
    public static char ToLower(this char c)
    {
        return char.ToLower(c);
    }

    /// <summary>
    /// Converts this <see cref="char"/> into its LowerCase equivalent.
    /// </summary>
    public static char ToLower(this char c, CultureInfo culture)
    {
        return char.ToLower(c, culture);
    }
}