using Jay.Dumping;

namespace Jay.Reflection.Exceptions;

public class ReflectionException : SystemException
{
    public ReflectionException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }

    public ReflectionException(ref InterpolatedDumpHandler message,
                               Exception? innerException = null)
        : base(message.ToStringAndDispose(), innerException)
    {

    }
}

public class RuntimeException : ReflectionException
{
    public RuntimeException(string? message = null, Exception? innerException = null) : base(message, innerException)
    {
    }
}