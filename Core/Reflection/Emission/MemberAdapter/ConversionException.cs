using System;

namespace Jay.Reflection.Emission
{
    public class ConversionException : Exception
    {
        /// <inheritdoc />
        public ConversionException(string? message = null, 
                                   Exception? innerException = null) 
            : base(message, innerException)
        {
        }
    }
}