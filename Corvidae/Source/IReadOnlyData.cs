using System;

namespace Corvidae
{
    /// <summary>
    /// Configured data for <see cref="ISource"/>
    /// </summary>
    public interface IReadOnlyData
    {
        /// <summary>
        /// Is there data associated with the <paramref name="key"/>?
        /// </summary>
        /// <param name="key">
        /// The <see cref="string"/>? key to check for associated data
        /// </param>
        /// <returns>
        /// True if there is data associated; otherwise, false
        /// </returns>
        bool Contains(string? key);
        
        /// <summary>
        /// Tries to get a <typeparamref name="TValue"/> <paramref name="value"/> for a <paramref name="key"/>
        /// </summary>
        /// <typeparam name="TValue">
        /// The <see cref="Type"/> of value that must be associated with the <paramref name="key"/>
        /// </typeparam>
        /// <param name="key">
        /// The <see cref="string"/>? key to check for a <paramref name="value"/>
        /// </param>
        /// <param name="value">
        /// The <paramref name="key"/>'s associated <typeparamref name="TValue"/> or <see langword="default{T}"/>
        /// </param>
        /// <returns>
        /// True if a <typeparamref name="TValue"/> <paramref name="value"/> was associated with the <paramref name="key"/>;
        /// otherwise, false.
        /// </returns>
        bool TryGetValue<TValue>(string? key, out TValue? value);
        
        /// <summary>
        /// Gets the <typeparamref name="TValue"/> value associated with the <paramref name="key"/> or a
        /// <paramref name="@default"/> value if one is not found 
        /// </summary>
        /// <typeparam name="TValue">
        /// The <see cref="Type"/> of value that must be associated with the <paramref name="key"/>
        /// </typeparam>
        /// <param name="key">
        /// The <see cref="string"/>? key to check for a <typeparamref name="TValue"/> value
        /// </param>
        /// <param name="default">
        /// The <typeparamref name="TValue"/> to return if nothing is associated with <paramref name="key"/>
        /// </param>
        /// <returns>
        /// The associated <typeparamref name="TValue"/> or <paramref name="@default"/>
        /// </returns>
        TValue? GetOrDefault<TValue>(string? key, TValue? @default = default(TValue));
    }
}