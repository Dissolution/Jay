using System.Collections;
using Jay.Reflection;
using Jay.Utilities;

namespace Jay;

/// <summary>
/// Represents the typed result of an operation as either:<br/>
/// <c>Ok(<typeparamref name="TValue">Value</typeparamref>)</c><br/>
/// <c>Error(<typeparamref name="TException">Exception</typeparamref>)</c>
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of Value stored with an <c>Ok</c> Result
/// </typeparam>
/// <typeparam name="TException">
/// The <see cref="Type"/> of <see cref="Exception"/> stored with an <c>Error</c> Result 
/// </typeparam>
public readonly struct Result<TValue, TException> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TValue, TException>, Result<TValue, TException>, bool>,
    IEqualityOperators<Result<TValue, TException>, Result<TValue>, bool>,
    IEqualityOperators<Result<TValue, TException>, Result, bool>,
    IEqualityOperators<Result<TValue, TException>, bool, bool>,
    IBitwiseOperators<Result<TValue, TException>, Result<TValue, TException>, bool>,
    IBitwiseOperators<Result<TValue, TException>, Result<TValue>, bool>,
    IBitwiseOperators<Result<TValue, TException>, Result, bool>,
    IBitwiseOperators<Result<TValue, TException>, bool, bool>,
#endif
    IEquatable<Result<TValue, TException>>,
    IEquatable<Result<TValue>>,
    IEquatable<Result>,
    IEnumerable<TValue>
    where TException : Exception
{
    public static implicit operator Result<TValue, TException>(TValue value) => Ok(value);
    public static implicit operator Result<TValue, TException>([NotNull] TException exception) => Error(exception);
    public static implicit operator bool(Result<TValue, TException> result) => result._ok;
    public static implicit operator Result(Result<TValue, TException> result) => new(result._ok, result._exception);
    public static implicit operator Result<TValue>(Result<TValue, TException> result) => new(result._ok, result._value, result._exception);

    public static bool operator true(Result<TValue, TException> result) => result._ok;
    public static bool operator false(Result<TValue, TException> result) => !result._ok;
    public static bool operator !(Result<TValue, TException> result) => !result._ok;
    public static bool operator ~(Result<TValue, TException> _)
    {
        throw new NotSupportedException($"Cannot apply ~ to a Result<{TypeNames.ToCode<TValue>()},{TypeNames.ToCode<TException>()}>");
    }

    public static bool operator ==(Result<TValue, TException> left, Result<TValue, TException> right) => left.Equals(right);
    public static bool operator ==(Result<TValue, TException> fullResult, Result<TValue> valueResult) => fullResult.Equals(valueResult);
    public static bool operator ==(Result<TValue, TException> fullResult, Result result) => fullResult.Equals(result);
    public static bool operator ==(Result<TValue, TException> fullResult, TValue value) => fullResult.Equals(value);
    public static bool operator ==(Result<TValue, TException> fullResult, [NotNull] TException error) => fullResult.Equals(error);
    public static bool operator ==(Result<TValue, TException> fullResult, bool pass) => fullResult.Equals(pass);
    public static bool operator ==(Result<TValue> valueResult, Result<TValue, TException> fullResult) => fullResult.Equals(valueResult);
    public static bool operator ==(Result result, Result<TValue, TException> fullResult) => fullResult.Equals(result);
    public static bool operator ==(TValue value, Result<TValue, TException> fullResult) => fullResult.Equals(value);
    public static bool operator ==([NotNull] TException error, Result<TValue, TException> fullResult) => fullResult.Equals(error);
    public static bool operator ==(bool pass, Result<TValue, TException> fullResult) => fullResult.Equals(pass);

    public static bool operator !=(Result<TValue, TException> left, Result<TValue, TException> right) => !left.Equals(right);
    public static bool operator !=(Result<TValue, TException> fullResult, Result<TValue> valueResult) => !fullResult.Equals(valueResult);
    public static bool operator !=(Result<TValue, TException> fullResult, Result result) => !fullResult.Equals(result);
    public static bool operator !=(Result<TValue, TException> fullResult, TValue value) => !fullResult.Equals(value);
    public static bool operator !=(Result<TValue, TException> fullResult, [NotNull] TException error) => !fullResult.Equals(error);
    public static bool operator !=(Result<TValue, TException> fullResult, bool pass) => !fullResult.Equals(pass);
    public static bool operator !=(Result<TValue> valueResult, Result<TValue, TException> fullResult) => !fullResult.Equals(valueResult);
    public static bool operator !=(Result result, Result<TValue, TException> fullResult) => !fullResult.Equals(result);
    public static bool operator !=(TValue value, Result<TValue, TException> fullResult) => !fullResult.Equals(value);
    public static bool operator !=([NotNull] TException error, Result<TValue, TException> fullResult) => !fullResult.Equals(error);
    public static bool operator !=(bool pass, Result<TValue, TException> fullResult) => !fullResult.Equals(pass);

    public static bool operator |(Result<TValue, TException> left, Result<TValue, TException> right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result<TValue, TException> fullResult, Result<TValue> valueResult) => fullResult.IsOk() || valueResult.IsOk();
    public static bool operator |(Result<TValue, TException> fullResult, Result result) => fullResult.IsOk() || result.IsOk();
    public static bool operator |(Result<TValue, TException> fullResult, bool pass) => pass || fullResult.IsOk();
    public static bool operator |(Result<TValue> valueResult, Result<TValue, TException> fullResult) => fullResult.IsOk() || valueResult.IsOk();
    public static bool operator |(Result result, Result<TValue, TException> fullResult) => fullResult.IsOk() || result.IsOk();
    public static bool operator |(bool pass, Result<TValue, TException> fullResult) => pass || fullResult.IsOk();
    
    public static bool operator &(Result<TValue, TException> left, Result<TValue, TException> right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result<TValue, TException> fullResult, Result<TValue> valueResult) => fullResult.IsOk() && valueResult.IsOk();
    public static bool operator &(Result<TValue, TException> fullResult, Result result) => fullResult.IsOk() && result.IsOk();
    public static bool operator &(Result<TValue, TException> fullResult, bool pass) => pass && fullResult.IsOk();
    public static bool operator &(Result<TValue> valueResult, Result<TValue, TException> fullResult) => fullResult.IsOk() && valueResult.IsOk();
    public static bool operator &(Result result, Result<TValue, TException> fullResult) => fullResult.IsOk() && result.IsOk();
    public static bool operator &(bool pass, Result<TValue, TException> fullResult) => pass && fullResult.IsOk();

    public static bool operator ^(Result<TValue, TException> left, Result<TValue, TException> right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result<TValue, TException> fullResult, Result<TValue> valueResult) => fullResult.IsOk() ^ valueResult.IsOk();
    public static bool operator ^(Result<TValue, TException> fullResult, Result result) => fullResult.IsOk() ^ result.IsOk();
    public static bool operator ^(Result<TValue, TException> fullResult, bool pass) => pass ^ fullResult.IsOk();
    public static bool operator ^(Result<TValue> valueResult, Result<TValue, TException> fullResult) => fullResult.IsOk() ^ valueResult.IsOk();
    public static bool operator ^(Result result, Result<TValue, TException> fullResult) => fullResult.IsOk() ^ result.IsOk();
    public static bool operator ^(bool pass, Result<TValue, TException> fullResult) => pass ^ fullResult.IsOk();

    /// <summary>
    /// Get a successful <see cref="Result{T}"/> with the given <paramref name="value"/>
    /// </summary>
    public static Result<TValue, TException> Ok(TValue value)
    {
        return new(true, value, null);
    }

    
    public static Result<TValue, TException> Error([NotNull] TException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        return new(false, default!, exception);
    }

    
    private readonly bool _ok;
    private readonly TValue _value;
    private readonly TException? _exception;

    internal Result(bool ok, TValue value, TException? exception)
    {
        _ok = ok;
        _value = value;
        _exception = exception;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TException GetException()
    {
        return _exception ?? Activator.CreateInstance<TException>();
    }

    /// <summary>
    /// If this <see cref="Result{TValue,TException}"/> is <c>Ok</c>, returns the <c>Value</c>,<br/>
    /// otherwise <c>throws</c> the <c>Error</c>'s <see cref="Exception"/>
    /// </summary>
    /// <returns>
    /// The Value from <c>Ok(Value)</c>
    /// </returns>
    /// <exception cref="Exception">
    /// The Exception from <c>Error(Exception)</c>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue OkOrThrowError() => _ok ? _value : throw GetException();

    /// <summary>
    /// If this <see cref="Result{TValue,TException}"/> is <c>Error(Exception)</c>, <c>throw Exception</c>
    /// </summary>
    /// <exception cref="Exception">
    /// The Exception from <c>Error(Exception)</c>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfError()
    {
        if (!_ok)
            throw GetException();
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
    /// Is this a failed <see cref="Result"/>?
    /// </summary>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_ok;

    /// <summary>
    /// Is this a failed <see cref="Result"/>?
    /// </summary>
    /// <param name="error">If this is a failed <see cref="Result"/>, the attached <see cref="Exception"/>; otherwise <see langword="null"/></param>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([NotNullWhen(true)] out TException? error)
    {
        if (_ok)
        {
            error = null;
            return false;
        }

        error = GetException();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<TValue> onOk, Action<TException> onError)
    {
        if (_ok)
        {
            onOk(_value);
        }
        else
        {
            onError(GetException());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<TValue, TReturn> onOk, Func<TException, TReturn> onError)
    {
        if (_ok)
        {
            return onOk(_value);
        }
        else
        {
            return onError(GetException());
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
    
    public bool Equals(Result<TValue, TException> fullResult)
    {
        if (_ok)
        {
            return fullResult.IsOk(out var value) && EqualityComparer<TValue>.Default.Equals(_value!, value!);
        }
        return fullResult.IsError();
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
    public bool Equals(TException? _) => !_ok;
    public bool Equals(Exception? _) => !_ok;
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<TValue, TException> fullResult => Equals(fullResult),
            Result<TValue> valueResult => Equals(valueResult),
            Result result => Equals(result),
            TValue value => Equals(value),
            TException ex => Equals(ex),
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
            error => $"Error({TypeNames.ToCode<TException>()}): {error.Message})");
    }
}