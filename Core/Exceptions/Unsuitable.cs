using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Exceptions;

public class UnsuitableException : InvalidOperationException
{
    public static UnsuitableException ForGetHashCode<T>(T? instance = default)
    {
        return new UnsuitableForGetHashCodeException(typeof(T));
    }

    /// <summary>
    /// Throws a new <see cref="UnsuitableForGetHashCodeException"/>
    /// </summary>
    [DoesNotReturn]
    public static int ThrowGetHashCode<T>(T? instance = default)
    {
        throw new UnsuitableForGetHashCodeException(typeof(T));
    }

    /// <summary>
    /// Throws a new <see cref="UnsuitableForGetHashCodeException"/> for the given <paramref name="instanceType"/>
    /// </summary>
    [DoesNotReturn]
    public static int ThrowGetHashCode(Type instanceType)
    {
        throw new UnsuitableForGetHashCodeException(instanceType);
    }

    public static UnsuitableException ForEquals<T>(T? instance = default)
    {
        return new UnsuitableException(typeof(T), $"Type '{typeof(T)}' is unsuitable for comparision");
    }

    /// <summary>
    /// Throws a new <see cref="UnsuitableForGetHashCodeException"/>
    /// </summary>
    [DoesNotReturn]
    public static bool ThrowEquals<T>(T? instance = default)
    {
        throw new UnsuitableForComparisonException(typeof(T));
    }

    /// <summary>
    /// Throws a new <see cref="UnsuitableForGetHashCodeException"/> for the given <paramref name="instanceType"/>
    /// </summary>
    [DoesNotReturn]
    public static bool ThrowEquals(Type instanceType)
    {
        throw new UnsuitableForComparisonException(instanceType);
    }

    public Type InstanceType { get; }

    protected UnsuitableException(Type instanceType,
                                  string? message,
                                  Exception? innerException = null)
        : base(message, innerException)
    {
        this.InstanceType = instanceType;
    }
}

/// <summary>
/// An exception to be thrown when a value does not support <see cref="M:GetHashCode()"/>.
/// </summary>
public class UnsuitableForGetHashCodeException : UnsuitableException
{
    /// <summary>
    /// Construct a <see cref="UnsuitableForGetHashCodeException"/> given the <paramref name="type"/> of instance that cannot be used
    /// for a call to <see cref="M:GetHashCode"/>
    /// </summary>
    /// <param name="instanceType">The type of instance that cannot generate a hash code</param>
    /// <param name="innerException">The optional <see cref="Exception"/> to store as our <see cref="P:InnerException"/></param>
    public UnsuitableForGetHashCodeException(Type instanceType, Exception? innerException = null)
        : base(instanceType, $"Type '{instanceType}' is unsuitable for GetHashCode()", innerException)
    {

    }
}

public class UnsuitableForComparisonException : UnsuitableException
{
    /// <summary>
    /// Construct a <see cref="UnsuitableForComparisonException"/> given the <paramref name="type"/> of instance that cannot be used
    /// for a call to <see cref="M:GetHashCode"/>
    /// </summary>
    /// <param name="instanceType">The type of instance that cannot generate a hash code</param>
    /// <param name="innerException">The optional <see cref="Exception"/> to store as our <see cref="P:InnerException"/></param>
    public UnsuitableForComparisonException(Type instanceType, Exception? innerException = null)
        : base(instanceType, $"Type '{instanceType}' is unsuitable for comparision", innerException)
    {

    }
}