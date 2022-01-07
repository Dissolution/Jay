namespace Jay.Reflection;

public class AdapterException : ReflectionException
{
    public AdapterException(string? message = null, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}