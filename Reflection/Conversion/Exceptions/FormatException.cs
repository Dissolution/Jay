namespace Jay.Reflection.Conversion.Exceptions;

public class FormatException : ConversionException
{
    public FormatException(Type? inputType, string? message = null, Exception? innerException = null)
        : base(inputType, typeof(string), message, innerException)
    {

    }
}