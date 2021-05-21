using System;
using System.Globalization;

namespace Jay
{
    /// <summary>
    /// Extensions for <see cref="char"/> and <see cref="Nullable"/>&lt;<see cref="char"/>&gt;s.
    /// </summary>
    public static class CharacterExtensions
    {
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
        /// Converts this <see cref="char"/> into its UpperCase equivelent.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static char ToUpper(this char c) => char.ToUpper(c);

        /// <summary>
        /// Converts this <see cref="char"/> into its UpperCase equivelent.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c, culture);

        /// <summary>
        /// Converts this <see cref="char"/> into its LowerCase equivelent.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static char ToLower(this char c) => char.ToLower(c);

        /// <summary>
        /// Converts this <see cref="char"/> into its LowerCase equivelent.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static char ToLower(this char c, CultureInfo culture) => char.ToLower(c, culture);
    }
}