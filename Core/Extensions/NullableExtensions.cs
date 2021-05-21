using System;

namespace Jay
{
    /// <summary>
    /// Extensions for <see cref="Nullable{T}"/>s.
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Try to get the non-null value inside this <see cref="Nullable{T}"/>.
        /// </summary>
        public static bool TryGetValue<T>(this T? nullable, out T value)
            where T : struct
        {
            value = nullable.GetValueOrDefault();
            return nullable.HasValue;
        }
    }
}