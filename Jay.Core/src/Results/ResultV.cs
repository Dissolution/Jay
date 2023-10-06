using System.Collections;
using Jay.Reflection;
using Jay.Utilities;

namespace Jay;

/// <summary>
/// Represents the typed result of an operation as either:<br/>
/// <c>Ok(<typeparamref name="TValue">Value</typeparamref>)</c><br/>
/// <c>Error(<see cref="Exception"/>)</c>
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of Value stored with an <c>Ok</c> Result
/// </typeparam>
public readonly struct Result<TValue> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TValue>, Result<TValue>, bool>,
    IEqualityOperators<Result<TValue>, Result, bool>,
    IEqualityOperators<Result<TValue>, bool, bool>,
    IBitwiseOperators<Result<TValue>, Result<TValue>, bool>,
    IBitwiseOperators<Result<TValue>, Result, bool>,
    IBitwiseOperators<Result<TValue>, bool, bool>,
#endif
    IEquatable<Result<TValue>>,
    IEquatable<Result>,
    IEnumerable<TValue>
{
    public static implicit operator Result<TValue>(TValue value) => Ok(value);
    public static implicit operator Result<TValue>(Exception? exception) => Error(exception);
    public static implicit operator bool(Result<TValue> result) => result._ok;
    public static implicit operator Result(Result<TValue> result) => new(result._ok, result._exception);

    public static bool operator true(Result<TValue> result) => result._ok;
    public static bool operator false(Result<TValue> result) => !result._ok;
    public static bool operator !(Result<TValue> result) => !result._ok;

    public static bool operator ~(Result<TValue> _)
    {
        throw new NotSupportedException($"Cannot apply ~ to a Result<{TypeNames.ToCode<TValue>()}>");
    }

    public static bool operator ==(Result<TValue> left, Result<TValue> right) => left.Equals(right);
    public static bool operator ==(Result<TValue> valueResult, Result result) => valueResult.Equals(result);
    public static bool operator ==(Result<TValue> valueResult, TValue value) => valueResult.Equals(value);
    public static bool operator ==(Result<TValue> valueResult, Exception? error) => valueResult.Equals(error);
    public static bool operator ==(Result<TValue> valueResult, bool pass) => valueResult.Equals(pass);
    public static bool operator ==(Result result, Result<TValue> valueResult) => valueResult.Equals(result);
    public static bool operator ==(TValue value, Result<TValue> valueResult) => valueResult.Equals(value);
    public static bool operator ==(Exception? error, Result<TValue> valueResult) => valueResult.Equals(error);
    public static bool operator ==(bool pass, Result<TValue> valueResult) => valueResult.Equals(pass);

    public static bool operator !=(Result<TValue> left, Result<TValue> right) => !left.Equals(right);
    public static bool operator !=(Result<TValue> valueResult, Result result) => !valueResult.Equals(result);
    public static bool operator !=(Result<TValue> valueResult, TValue value) => !valueResult.Equals(value);
    public static bool operator !=(Result<TValue> valueResult, Exception? error) => !valueResult.Equals(error);
    public static bool operator !=(Result<TValue> valueResult, bool pass) => !valueResult.Equals(pass);
    public static bool operator !=(Result result, Result<TValue> valueResult) => !valueResult.Equals(result);
    public static bool operator !=(TValue value, Result<TValue> valueResult) => !valueResult.Equals(value);
    public static bool operator !=(Exception? error, Result<TValue> valueResult) => !valueResult.Equals(error);
    public static bool operator !=(bool pass, Result<TValue> valueResult) => !valueResult.Equals(pass);

    public static bool operator |(Result<TValue> left, Result<TValue> right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result<TValue> valueResult, Result result) => valueResult.IsOk() || result.IsOk();
    public static bool operator |(Result<TValue> valueResult, bool pass) => pass || valueResult.IsOk();
    public static bool operator |(Result result, Result<TValue> valueResult) => valueResult.IsOk() || result.IsOk();
    public static bool operator |(bool pass, Result<TValue> valueResult) => pass || valueResult.IsOk();

    public static bool operator &(Result<TValue> left, Result<TValue> right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result<TValue> valueResult, Result result) => valueResult.IsOk() && result.IsOk();
    public static bool operator &(Result<TValue> valueResult, bool pass) => pass && valueResult.IsOk();
    public static bool operator &(Result result, Result<TValue> valueResult) => valueResult.IsOk() && result.IsOk();
    public static bool operator &(bool pass, Result<TValue> valueResult) => pass && valueResult.IsOk();

    public static bool operator ^(Result<TValue> left, Result<TValue> right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result<TValue> valueResult, Result result) => valueResult.IsOk() ^ result.IsOk();
    public static bool operator ^(Result<TValue> valueResult, bool pass) => pass ^ valueResult.IsOk();
    public static bool operator ^(Result result, Result<TValue> valueResult) => valueResult.IsOk() ^ result.IsOk();
    public static bool operator ^(bool pass, Result<TValue> valueResult) => pass ^ valueResult.IsOk();

    /// <summary>
    /// Returns a passing <see cref="Result{T}" /> with the given <paramref name="value" />.
    /// </summary>
    /// <param name="value">The passing value.</param>
    /// <returns>A passing <see cref="Result{T}" />.</returns>
    public static Result<TValue> Ok(TValue value)
    {
        return new(true, value, null);
    }

    /// <summary>
    /// Returns a failing <see cref="Result{T}" /> with the given <paramref name="error" />.
    /// </summary>
    /// <param name="error">The failing <see cref="_exception" />.</param>
    /// <returns>A failed <see cref="Result{T}" />.</returns>
    public static Result<TValue> Error(Exception? error)
    {
        return new(false, default!, error);
    }


    private readonly bool _ok;
    private readonly TValue _value;
    private readonly Exception? _exception;

    internal Result(bool ok, TValue value, Exception? exception)
    {
        _ok = ok;
        _value = value;
        _exception = exception;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception GeException()
    {
        return _exception ?? new Exception();
    }

    /// <summary>
    /// If this <see cref="Result{TValue}"/> is <c>Ok</c>, returns the <c>Value</c>,<br/>
    /// otherwise <c>throws</c> the <c>Error</c>'s <see cref="Exception"/>
    /// </summary>
    /// <returns>
    /// The Value from <c>Ok(Value)</c>
    /// </returns>
    /// <exception cref="Exception">
    /// The Exception from <c>Error(Exception)</c>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue OkValueOrThrowError() => _ok ? _value : throw GeException();

    /// <summary>
    /// If this <see cref="Result{TValue}"/> is <c>Error(Exception)</c>, <c>throw Exception</c>
    /// </summary>
    /// <exception cref="Exception">
    /// The Exception from <c>Error(Exception)</c>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfError()
    {
        if (!_ok)
            throw GeException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _ok;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out TValue value)
    {
        value = _value;
        return _ok;
    }

    /// <summary>
    /// Is this a failed <see cref="Result" />?
    /// </summary>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_ok;

    /// <summary>
    /// Is this a failed <see cref="Result" />?
    /// </summary>
    /// <param name="error">If this is a failed <see cref="Result" />, the attached <see cref="Exception" />; otherwise <see langword="null" /></param>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([NotNullWhen(true)] out Exception? error)
    {
        if (_ok)
        {
            error = null;
            return false;
        }

        error = GeException();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<TValue> onOk, Action<Exception> onError)
    {
        if (_ok)
        {
            onOk(_value);
        }
        else
        {
            onError(GeException());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<TValue, TReturn> onOk, Func<Exception, TReturn> onError)
    {
        if (_ok)
        {
            return onOk(_value);
        }
        else
        {
            return onError(GeException());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TValue> AsOption()
    {
        return _ok ? Option<TValue>.Some(_value) : Option<TValue>.None;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TValue> GetEnumerator()
    {
        if (_ok)
        {
            yield return _value;
        }
    }

    public bool Equals(Result<TValue> valueResult)
    {
        if (_ok)
        {
            return valueResult.IsOk(out var value) && EqualityComparer<TValue>.Default.Equals(_value!, value!);
        }
        return valueResult.IsError();
    }

    public bool Equals(Result result) => _ok == result.IsOk();
    public bool Equals(TValue? value) => _ok && EqualityComparer<TValue>.Default.Equals(_value!, value!);
    public bool Equals(Exception? _) => !_ok;
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<TValue> fullResult => Equals(fullResult),
            Result result => Equals(result),
            TValue value => Equals(value),
            Exception ex => Equals(ex),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _ok ? Hasher.GetHashCode<TValue>(_value) : 0;

    public override string ToString()
    {
        return Match(
            value => $"Ok({TypeNames.ToCode<TValue>()}: {value})",
            error => $"Error({error.GetType().ToCode()}): {error.Message})");
    }
}