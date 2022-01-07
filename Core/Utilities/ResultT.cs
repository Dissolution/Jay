using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Jay;

/// <summary>
/// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
/// </summary>
public readonly struct Result<T> : IEquatable<Result<T>>,
                                   IEquatable<T?>,
                                   IEnumerable<T?>
{
    #region Static
    public static implicit operator Result<T>(T? value) => new Result<T>(true, value, null);
    public static implicit operator Result<T>(Exception? exception) => new Result<T>(false, default(T), exception ?? new Exception(Result.DefaultErrorMessage));
    public static implicit operator bool(Result<T> result) => result._pass;
    public static explicit operator T?(Result<T> result) => result.GetValue();
    public static explicit operator Exception?(Result<T> result) => result._pass ? null : result._error ?? new Exception(Result.DefaultErrorMessage);

    public static bool operator ==(Result<T> x, Result<T> y) => x._pass == y._pass;
    public static bool operator !=(Result<T> x, Result<T> y) => x._pass != y._pass;
    public static bool operator ==(Result<T> x, Result y) => x._pass == y._pass;
    public static bool operator !=(Result<T> x, Result y) => x._pass != y._pass;
    public static bool operator ==(Result<T> x, bool y) => x._pass == y;
    public static bool operator !=(Result<T> x, bool y) => x._pass != y;

    public static bool operator |(Result<T> x, Result<T> y) => x._pass || y._pass;
    public static bool operator |(Result<T> x, Result y) => x._pass || y._pass;
    public static bool operator |(Result<T> x, bool y) => y || x._pass;
    public static bool operator &(Result<T> x, Result<T> y) => x._pass && y._pass;
    public static bool operator &(Result<T> x, Result y) => x._pass && y._pass;
    public static bool operator &(Result<T> x, bool y) => y && x._pass;
    public static bool operator ^(Result<T> x, Result<T> y) => x._pass ^ y._pass;
    public static bool operator ^(Result<T> x, Result y) => x._pass ^ y._pass;
    public static bool operator ^(Result<T> x, bool y) => x._pass ^ y;

    public static bool operator true(Result<T> result) => result._pass;
    public static bool operator false(Result<T> result) => !result._pass;
    public static bool operator !(Result<T> result) => !result._pass;

    /// <summary>
    /// Returns a passing <see cref="Result{T}"/> with the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The passing value.</param>
    /// <returns>A passing <see cref="Result{T}"/>.</returns>
    public static Result<T> Pass(T? value) => new Result<T>(true, value, null);

    /// <summary>
    /// Returns a failing <see cref="Result{T}"/> with the given <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The failing <see cref="_error"/>.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Fail(Exception? exception = null) => new Result<T>(false, default(T), exception ?? new Exception(Result.DefaultErrorMessage));

    /// <summary>
    /// Try to execute the given <paramref name="func"/>, attempt to store its result in <paramref name="value"/>,
    /// and return a <see cref="Result"/>.
    /// </summary>
    /// <param name="func">The function to try to execute.</param>
    /// <param name="value">The return value of <paramref name="func"/> or <see langword="default{T}"/> if an error occurred.</param>
    /// <returns>
    /// A passing <see cref="Result"/> if the <paramref name="func"/> executed successfully or a failed one containing
    /// the caught error if it did not.
    /// </returns>
    public static Result Try(Func<T>? func, out T? value) => Result.Try<T>(func, out value);

    /// <summary>
    /// Try to execute the given <paramref name="func"/> and return a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="func">The function to try to execute.</param>
    /// <returns>
    /// A passing <see cref="Result{T}"/> containing <paramref name="func"/>'s return value or
    /// a failing <see cref="Result{T}"/> containing the captured <see cref="_error"/>.
    /// </returns>
    public static Result<T> Try(Func<T?>? func)
    {
        if (func is null)
        {
            return new ArgumentNullException(nameof(func));
        }
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    #endregion

    internal readonly bool _pass;
    internal readonly T? _value;
    internal readonly Exception? _error;

    internal Result(bool pass, T? value, Exception? error)
    {
        _pass = pass;
        _value = value;
        _error = error;
    }

    internal T? GetValue()
    {
        if (_pass)
            return _value;
        throw new InvalidOperationException("A failed Result has no Value");
    }

    public Result TryGetValue(out T? value)
    {
        value = _value;
        return new Result(_pass, _error);
    }

    public void ThrowIfFailed()
    {
        if (!_pass)
        {
            throw _error ?? new Exception(Result.DefaultErrorMessage);
        }
    }

    public bool Failed() => !_pass;

    public bool Failed([NotNullWhen(true)] out Exception? error)
    {
        if (_pass)
        {
            error = null;
            return false;
        }

        error = (_error ?? new Exception(Result.DefaultErrorMessage));
        return true;
    }

    public bool Equals(Result<T> result)
    {
        if (_pass)
        {
            if (result._pass)
            {
                return EqualityComparer<T>.Default.Equals(_value, result._value);
            }
            return false;
        }
        return result._pass == false;
    }

    public bool Equals(Result result) => result._pass == _pass;

    public bool Equals(bool pass) => pass == _pass;

    public bool Equals(T? value) => _pass && EqualityComparer<T>.Default.Equals(_value, value);

    public IEnumerator<T?> GetEnumerator()
    {
        if (_pass)
        {
            yield return _value;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals(object? obj)
    {
        if (obj is Result<T> resultT)
            return Equals(resultT);
        if (obj is Result result)
            return Equals(result);
        if (obj is bool pass)
            return Equals(pass);
        if (obj is T value)
            return Equals(value);
        return false;
    }

    public override int GetHashCode()
    {
        if (_pass)
        {
            return HashCode.Combine(1, _value);
        }
        else
        {
            return HashCode.Combine(0, default(T));
        }
    }

    public override string ToString()
    {
        if (_pass)
            return $"Pass: {_value}";
        if (_error is null)
            return "Fail";
        return $"Fail: {_error}";
    }
}