#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay;

public readonly partial struct Result<T>
#if NET7_0_OR_GREATER
    : IEqualityOperators<Result<T>, Result<T>, bool>,
        IEqualityOperators<Result<T>, Result, bool>,
        IEqualityOperators<Result<T>, bool, bool>,
        IBitwiseOperators<Result<T>, Result<T>, bool>,
        IBitwiseOperators<Result<T>, Result, bool>,
        IBitwiseOperators<Result<T>, bool, bool>
#endif
{
    public static implicit operator Result<T>(T value)
    {
        return Ok(value);
    }
    public static implicit operator Result<T>(Exception? error)
    {
        return Error(error);
    }
    public static implicit operator bool(Result<T> result)
    {
        return result._ok;
    }
    public static implicit operator Result(Result<T> result)
    {
        return new(result._ok, result._error);
    }

    public static bool operator true(Result<T> result)
    {
        return result._ok;
    }
    public static bool operator false(Result<T> result)
    {
        return !result._ok;
    }
    public static bool operator !(Result<T> result)
    {
        return !result._ok;
    }
    public static bool operator ~(Result<T> value)
    {
        throw new NotSupportedException("Cannot apply ~ to Result");
    }

    public static bool operator ==(Result<T> x, Result<T> y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result<T> x, Result y)
    {
        return x._ok == y._ok;
    }
    public static bool operator ==(Result<T> result, bool pass)
    {
        return result._ok == pass;
    }
    public static bool operator !=(Result<T> x, Result<T> y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result<T> x, Result y)
    {
        return x._ok != y._ok;
    }
    public static bool operator !=(Result<T> result, bool pass)
    {
        return result._ok != pass;
    }

    public static bool operator |(Result<T> x, Result<T> y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result<T> x, Result y)
    {
        return x._ok || y._ok;
    }
    public static bool operator |(Result<T> result, bool pass)
    {
        return pass || result._ok;
    }
    public static bool operator &(Result<T> x, Result<T> y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result<T> x, Result y)
    {
        return x._ok && y._ok;
    }
    public static bool operator &(Result<T> result, bool pass)
    {
        return pass && result._ok;
    }
    public static bool operator ^(Result<T> x, Result<T> y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result<T> x, Result y)
    {
        return x._ok ^ y._ok;
    }
    public static bool operator ^(Result<T> result, bool pass)
    {
        return result._ok ^ pass;
    }


    /// <summary>
    /// Returns a passing <see cref="Result{T}" /> with the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">The passing value.</param>
    /// <returns>A passing <see cref="Result{T}" />.</returns>
    public static Result<T> Ok(T value)
    {
        return new(true, value, null);
    }

    /// <summary>
    /// Returns a failing <see cref="Result{T}" /> with the given <paramref name="error" />.
    /// </summary>
    /// <param name="error">The failing <see cref="_error" />.</param>
    /// <returns>A failed <see cref="Result{T}" />.</returns>
    public static Result<T> Error(Exception? error = null)
    {
        return new(false, default, error ?? new Exception(Result.DefaultErrorMessage));
    }
}