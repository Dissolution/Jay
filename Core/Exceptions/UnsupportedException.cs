namespace Jay.Exceptions;

public class UnsupportedException : NotSupportedException
{
    [DoesNotReturn]
    public static bool ThrowForEquals(Type instanceType)
    {
        throw new UnsupportedException($"{instanceType} does not support {nameof(Equals)}()");
    }

    [DoesNotReturn]
    public static bool ThrowForEquals<T>(T? _ = default)
    {
        throw new UnsupportedException($"{typeof(T)} does not support {nameof(Equals)}()");
    }

    [DoesNotReturn]
    public static int ThrowForGetHashCode(Type instanceType)
    {
        throw new UnsupportedException($"{instanceType} does not support {nameof(GetHashCode)}()");
    }

    [DoesNotReturn]
    public static int ThrowForGetHashCode<T>(T? _ = default)
    {
        throw new UnsupportedException($"{typeof(T)} does not support {nameof(GetHashCode)}()");
    }


    public UnsupportedException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}