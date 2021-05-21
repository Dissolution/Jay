using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    /// <summary>
    /// An exception to be thrown when a value does not support <see cref="M:GetHashCode()"/>.
    /// </summary>
    public class GetHashCodeException : InvalidOperationException
    {
        [DoesNotReturn]
        public static int Throw<T>(T? _ = default)
        {
            throw new GetHashCodeException(typeof(T));
        }
        
        [DoesNotReturn]
        public static int Throw(Type type)
        {
            throw new GetHashCodeException(type);
        }
        
        /// <summary>
        /// The <see cref="Type"/> of the struct or class that threw this exception.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// Construct a <see cref="GetHashCodeException"/> specifying the source's <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the struct or class that thew this exception.</param>
        public GetHashCodeException(Type type)
            : base($"Type '{type}' does not support GetHashCode()")
        {
            this.SourceType = type;
        }

        /// <summary>
        /// Construct a <see cref="GetHashCodeException"/> specifying the source's <paramref name="type"/> an an inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="type">The type of the struct or class that thew this exception.</param>
        /// <param name="inner">The exception to store as our <see cref="P:InnerException"/></param>
        public GetHashCodeException(Type type, Exception inner)
            : base($"Type '{type}' does not support GetHashCode()", inner)
        {
            this.SourceType = type;
        }
    }
}