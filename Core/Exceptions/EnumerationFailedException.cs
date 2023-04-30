namespace Jay.Exceptions;

public class EnumerationFailedException : Exception
{
    public EnumerationFailedException(
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}