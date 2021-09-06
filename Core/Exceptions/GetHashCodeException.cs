using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Exceptions
{
    /// <summary>
    /// An exception to be thrown when a value does not support <see cref="M:GetHashCode()"/>.
    /// </summary>
    public class GetHashCodeException : InvalidOperationException
    {
        /// <summary>
        /// Throws a new <see cref="GetHashCodeException"/>
        /// </summary>
        [DoesNotReturn]
        public static int Throw<T>(T? instance = default)
        {
            throw new GetHashCodeException(typeof(T));
        }
        
        /// <summary>
        /// Throws a new <see cref="GetHashCodeException"/> for the given <paramref name="type"/>
        /// </summary>
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
        /// Construct a <see cref="GetHashCodeException"/> given the <paramref name="type"/> of instance that cannot be used
        /// for a call to <see cref="M:GetHashCode"/>
        /// </summary>
        /// <param name="type">The type of instance that cannot generate a hash code</param>
        /// <param name="innerException">The optional <see cref="Exception"/> to store as our <see cref="P:InnerException"/></param>
        public GetHashCodeException(Type type, Exception? innerException = null)
            : base($"Type '{type}' does not support GetHashCode()", innerException)
        {
            this.SourceType = type;
        }
    }
}