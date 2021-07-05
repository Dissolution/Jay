using System;

namespace Corvidae
{
    /// <summary>
    /// A source for getting values
    /// </summary>
    public interface IValueSource : IDisposable
    {
        /// <summary>
        /// The <see cref="Type"/> of values that <see cref="Get"/> returns
        /// </summary>
        Type ValueType { get; }
        
        /// <summary>
        /// Gets a <see cref="ValueType"/> value from this source, using the 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        object? Get(ISource source);
    }
}