using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Debugging
{
    /// <summary>
    /// Debugging extensions on <typeparamref name="T"/> values.
    /// </summary>
    public static class TExtensions
    {
        [return: NotNull]
        public static T ThrowIfNull<T>([AllowNull, NotNull] this T value,
                                       string? paramName = "value",
                                       string? exceptionMessage = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, exceptionMessage);
            }
            return value;
        }
        
        [return: NotNull]
        public static string ThrowIfNullOrEmpty(this string? value,
                                                string? paramName = "value",
                                                string? exceptionMessage = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName, exceptionMessage);
            }
            return value;
        }
        
        [return: NotNull]
        public static string ThrowIfNullOrWhiteSpace(this string? value,
                                                     string? paramName = "value",
                                                     string? exceptionMessage = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName, exceptionMessage);
            }
            return value;
        }
    }
}