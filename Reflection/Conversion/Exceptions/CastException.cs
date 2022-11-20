namespace Jay.Reflection.Conversion.Exceptions;

public class CastException : ConversionException
{
    public CastException(Type? inputType, Type? outputType, string? message = null, Exception? innerException = null)
        : base(inputType, outputType, message, innerException)
    {

    }
}