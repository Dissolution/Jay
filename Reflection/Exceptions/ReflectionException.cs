namespace Jay.Reflection;

public class ReflectionException : SystemException
{
    public ReflectionException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}

public class RuntimeException : ReflectionException
{
    public RuntimeException(string? message = null, Exception? innerException = null) : base(message, innerException)
    {
    }
}