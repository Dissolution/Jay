using System.Globalization;
using Jay.Reflection;

namespace Jay;

public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> AsReadOnlySpan(ref this char ch)
    {
        // Tested fastest
        unsafe
        {
            return new ReadOnlySpan<char>(Danger.InToVoidPointer(in ch), 1);
        }
    }

    /// <summary>
    /// Is this <see cref="char"/> a digit?
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool IsDigit(this char c) => char.IsDigit(c);

    /// <summary>
    /// Is this <see cref="char"/> considered white-space?
    /// </summary>
    public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);

    /// <summary>
    /// Converts this <see cref="char"/> into its UpperCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char ToUpper(this char c) => char.ToUpper(c);

    /// <summary>
    /// Converts this <see cref="char"/> into its UpperCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c, culture);

    /// <summary>
    /// Converts this <see cref="char"/> into its LowerCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char ToLower(this char c) => char.ToLower(c);

    /// <summary>
    /// Converts this <see cref="char"/> into its LowerCase equivalent.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static char ToLower(this char c, CultureInfo culture) => char.ToLower(c, culture);
}

