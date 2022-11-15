using System.Diagnostics.CodeAnalysis;

namespace Jay.Exceptions;

public class UnsupportedException : NotSupportedException
{
    [DoesNotReturn]
    public static int ThrowForGetHashCode<T>(T? _ = default)
    {
        throw new UnsupportedException($"{typeof(T)} does not support {nameof(GetHashCode)}");
    }


    public UnsupportedException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}