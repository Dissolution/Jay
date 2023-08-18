#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay;

public readonly partial struct Result<T, E>
#if NET7_0_OR_GREATER
    : IEqualityOperators<Result<T, E>, Result<T, E>, bool>,
        IEqualityOperators<Result<T, E>, Result<T>, bool>,
        IEqualityOperators<Result<T, E>, Result, bool>,
        IEqualityOperators<Result<T, E>, bool, bool>,
        IBitwiseOperators<Result<T, E>, Result<T, E>, bool>,
        IBitwiseOperators<Result<T, E>, Result<T>, bool>,
        IBitwiseOperators<Result<T, E>, Result, bool>,
        IBitwiseOperators<Result<T, E>, bool, bool>
#endif
{
    public static implicit operator Result<T, E>(T value)
    {
        return Ok(value);
    }
    public static implicit operator Result<T, E>([NotNull] E error)
    {
        return Error(error);
    }

    public static implicit operator bool(Result<T, E> result)
    {
        return result._ok;
    }
    public static implicit operator Result(Result<T, E> result)
    {
        return new(result._ok, result._error);
    }
    public static implicit operator Result<T>(Result<T, E> result)
    {
        return new(result._ok, result._value, result._error);
    }

    public static bool operator true(Result<T, E> result)
    {
        return result._ok;
    }
    public static bool operator false(Result<T, E> result)
    {
        return !result._ok;
    }
    public static bool operator !(Result<T, E> result)
    {
        return !result._ok;
    }
    public static bool operator ~(Result<T, E> value)
    {
        throw new NotSupportedException("Cannot apply ~ to Result");
    }

    public static bool operator ==(Result<T, E> x, Result<T, E> y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result<T, E> x, Result<T> y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result<T, E> x, Result y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result<T, E> result, bool pass)
    {
        return result._ok == pass;
    }
    public static bool operator !=(Result<T, E> x, Result<T, E> y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result<T, E> x, Result<T> y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result<T, E> x, Result y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result<T, E> result, bool pass)
    {
        return result._ok != pass;
    }

    public static bool operator |(Result<T, E> x, Result<T, E> y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result<T, E> x, Result<T> y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result<T, E> x, Result y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result<T, E> result, bool pass)
    {
        return pass || result._ok;
    }
    public static bool operator &(Result<T, E> x, Result<T, E> y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result<T, E> x, Result<T> y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result<T, E> x, Result y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result<T, E> result, bool pass)
    {
        return pass && result._ok;
    }
    public static bool operator ^(Result<T, E> x, Result<T, E> y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result<T, E> x, Result<T> y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result<T, E> x, Result y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result<T, E> result, bool pass)
    {
        return result._ok ^ pass;
    }


    /// <summary>
    /// Returns a passing <see cref="Result{T}" /> with the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">The passing value.</param>
    /// <returns>A passing <see cref="Result{T}" />.</returns>
    public static Result<T, E> Ok(T value)
    {
        return new(true, value, null);
    }

    /// <summary>
    /// Returns a failing <see cref="Result{T}" /> with the given <paramref name="error" />.
    /// </summary>
    /// <param name="error">The failing <see cref="_error" />.</param>
    /// <returns>A failed <see cref="Result{T}" />.</returns>
    public static Result<T, E> Error([NotNull] E error)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error));
        return new(false, default, error);
    }
}