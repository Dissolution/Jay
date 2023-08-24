using System.Globalization;

namespace Jay.Extensions;

public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char" /> into a <see cref="ReadOnlySpan{T}" />
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    /// <remarks>
    /// We use `ref` (could also use `in`) so that we can capture a pointer to the char.
    /// If we do not, the below doesn't work.
    /// </remarks>
    public static ReadOnlySpan<char> AsSpan(in this char ch)
    {
#if NET7_0_OR_GREATER
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
    /// Is this <see cref="char" /> a digit?
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }

    /// <summary>
    /// Is this <see cref="char" /> considered white-space?
    /// </summary>
    public static bool IsWhiteSpace(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    /// <summary>
    /// Converts this <see cref="char" /> into its UpperCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char ToUpper(this char c)
    {
        return char.ToUpper(c);
    }

    /// <summary>
    /// Converts this <see cref="char" /> into its UpperCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static char ToUpper(this char c, CultureInfo culture)
    {
        return char.ToUpper(c, culture);
    }

    /// <summary>
    /// Converts this <see cref="char" /> into its LowerCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char ToLower(this char c)
    {
        return char.ToLower(c);
    }

    /// <summary>
    /// Converts this <see cref="char" /> into its LowerCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static char ToLower(this char c, CultureInfo culture)
    {
        return char.ToLower(c, culture);
    }
}