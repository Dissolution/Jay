using System;

namespace Jay.Reflection.Emission
{
    public class ConversionSafetyException : ConversionException
    {
        /// <inheritdoc />
        public ConversionSafetyException(string? message = null, 
                                         Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}