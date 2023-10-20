using Jay.Reflection;

namespace Jay;

/// <summary>
/// Represents the result of an operation as either:<br/>
/// <see cref="Ok">Result.Ok()</see><br/>
/// <see cref="Error">Result.Error(Exception)</see><br/>
/// </summary>
public readonly partial struct Result :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result, Result, bool>,
    IEqualityOperators<Result, bool, bool>,
    IBitwiseOperators<Result, Result, bool>,
    IBitwiseOperators<Result, bool, bool>,
#endif
    IEquatable<Result>,
    IEquatable<bool>
{
    public static implicit operator Result(bool isOk) => isOk ? _okResult : Error(null);
    public static implicit operator Result(Exception? error) => Error(error);
    public static implicit operator bool(Result result) => result._ok;

    public static bool operator true(Result result) => result._ok;
    public static bool operator false(Result result) => !result._ok;
    public static bool operator !(Result result) => !result._ok;
    [Obsolete("Do not use ~ with Result", true)]
    public static bool operator ~(Result _) => throw new NotSupportedException("Cannot apply ~ to a Result");

    public static bool operator ==(Result result, Result otherResult) => result.Equals(otherResult);
    public static bool operator ==(Result result, Exception? error) => result.Equals(error);
    public static bool operator ==(Exception? error, Result result) => result.Equals(error);
    public static bool operator ==(Result result, bool isOk) => result.Equals(isOk);
    public static bool operator ==(bool isOk, Result result) => result.Equals(isOk);

    public static bool operator !=(Result result, Result otherResult) => !result.Equals(otherResult);
    public static bool operator !=(Result result, Exception? error) => !result.Equals(error);
    public static bool operator !=(Exception? error, Result result) => !result.Equals(error);
    public static bool operator !=(Result result, bool isOk) => !result.Equals(isOk);
    public static bool operator !=(bool isOk, Result result) => !result.Equals(isOk);

    public static bool operator |(Result result, Result otherResult) => result._ok || otherResult._ok;
    public static bool operator |(Result result, bool isOk) => isOk || result._ok;
    public static bool operator |(bool isOk, Result result) => isOk || result._ok;

    public static bool operator &(Result result, Result otherResult) => result._ok && otherResult._ok;
    public static bool operator &(Result result, bool isOk) => isOk && result._ok;
    public static bool operator &(bool isOk, Result result) => isOk && result._ok;

    public static bool operator ^(Result left, Result right) => left._ok ^ right._ok;
    public static bool operator ^(Result result, bool isOk) => isOk ^ result._ok;
    public static bool operator ^(bool isOk, Result result) => isOk ^ result._ok;
    
    private static readonly Result _okResult = new Result(true, null);

    /// <summary>
    /// Returns an <c>Ok</c> <see cref="Result"/>
    /// </summary>
    /// <returns><c>Result.Ok()</c></returns>
    public static Result Ok() => _okResult;

    /// <summary>
    /// Returns an <c>Error</c> <see cref="Result"/> with the given <paramref name="exception"/>
    /// </summary>
    /// <param name="exception">
    /// An optional <see cref="Exception"/> with additional information related to the failure
    /// </param>
    /// <returns><c>Result.Error(</c><see cref="Exception"/><c>)</c></returns>
    public static Result Error(Exception? exception)
    {
        return new(false, exception);
    }

    /* We would ideally like to have an Exception associated with every Error,
     * but we can't stop someone from using default(Result).
     * So, we have to be sure that default(Result) == default(bool) == false
     * Given that, we cannot just use an Exception? field to determine ok/error,
     * as default(Result) would be null which would mean Ok
     * As now we have to account for a null Exception, we can then just let the user
     * give us null, using no additional resources, and then create an Exception only if
     * we have to
     */
    

    internal readonly bool _ok;
    internal readonly Exception? _exception;

    internal Result(bool ok, Exception? exception)
    {
        _ok = ok;
        _exception = exception;
    }

    /// <summary>
    /// Gets an attached <see cref="Exception"/> or creates a new <see cref="Exception"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception GetException()
    {
        return _exception ?? new Exception();
    }

    /// <summary>
    /// If this <see cref="Result"/> is an <c>Error(Exception)</c>, <c>throw</c> the <see cref="Exception"/>
    /// </summary>
    /// <exception cref="Exception">
    /// The Exception from <c>Error(Exception)</c> or a new <see cref="Exception"/> if one wasn't provided
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfError()
    {
        if (!_ok)
            throw GetException();
    }

    /// <summary>
    /// Is this an <c>Ok</c> <see cref="Result"/>?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is <c>Result.Ok()</c>; otherwise, <c>false</c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _ok;

    /// <summary>
    /// Is this an <c>Error</c> <see cref="Result"/>?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is <c>Result.Error(Exception)</c>; otherwise, <c>false</c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_ok;

    /// <summary>
    /// Is this an <c>Error</c> <see cref="Result"/>?
    /// </summary>
    /// <param name="exception">
    /// If this is an <c>Error</c>, the attached or new <see cref="Exception"/>; otherwise, <c>null</c>
    /// </param>
    /// <returns>
    /// <c>true</c> if this is <c>Result.Error(Exception)</c>; otherwise, <c>false</c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([NotNullWhen(true)] out Exception? exception)
    {
        if (_ok)
        {
            exception = null;
            return false;
        }
        exception = GetException();
        return true;
    }

    /// <summary>
    /// Performs a matching operation on this <see cref="Result"/>,
    /// executing <paramref name="onOk"/> if this is an <c>Ok</c> Result and
    /// executing <paramref name="onError"/> if this is an <c>Error</c> Result
    /// </summary>
    /// <param name="onOk">
    /// The <see cref="Action">Action</see> that will be invoked if this is <c>Result.Ok()</c>
    /// </param>
    /// <param name="onError">
    /// The <see cref="Action{Exception}">Action&lt;Exception&gt;</see> that will be invoked if this is <c>Result.Error(Exception)</c>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action onOk, Action<Exception> onError)
    {
        if (_ok)
        {
            onOk();
        }
        else
        {
            onError(GetException());
        }
    }

    /// <summary>
    /// Performs a matching operation on this <see cref="Result"/>,
    /// executing <paramref name="onOk"/> if this is an <c>Ok</c> Result and
    /// executing <paramref name="onError"/> if this is an <c>Error</c> Result,
    /// returning a <typeparamref name="TReturn"/> value
    /// </summary>
    /// <param name="onOk">
    /// The <see cref="Func{TResult}">Func&lt;TReturn&gt;</see> that will be invoked if this is <c>Result.Ok()</c>
    /// </param>
    /// <param name="onError">
    /// The <see cref="Func{Exception,TResult}">Func&lt;Exception,TReturn&gt;</see> that will be invoked if this is <c>Result.Error(Exception)</c>
    /// </param>
    /// <typeparam name="TReturn">
    /// The <see cref="Type"/> of the return value
    /// </typeparam>
    /// <returns>
    /// The <typeparamref name="TReturn"/> value from either <paramref name="onOk"/> or <paramref name="onError"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<TReturn> onOk, Func<Exception, TReturn> onError)
    {
        if (_ok)
        {
            return onOk();
        }
        else
        {
            return onError(GetException());
        }
    }

    /// <summary>
    /// Indicates if this <see cref="Result"/> and the given <paramref name="result"/> are both <c>Ok</c> or <c>Error</c>
    /// </summary>
    public bool Equals(Result result) => _ok == result.IsOk();
    
    /// <summary>
    /// Indicates if this <see cref="Result"/> is an <c>Error</c>
    /// </summary>
    /// <param name="_">
    /// Ignored <see cref="Exception"/>, all <c>Result.Error(Exception)</c>s are the same
    /// </param>
    public bool Equals(Exception? _) => !_ok;

    /// <summary>
    /// <paramref name="isOk"/><c> == true</c>: Is this <c>Result.Ok()</c>?<br/>
    /// <paramref name="isOk"/><c> == false</c>: Is this <c>Result.Error(Exception)</c>?
    /// </summary>
    /// <param name="isOk">If this <see cref="Result"/> must be <c>Ok</c> or <c>Error</c></param>
    /// <returns>
    /// <c>true</c> if this is <c>Result.Ok()</c>; otherwise, <c>false</c>
    /// </returns>
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result result => Equals(result),
            Exception exception => Equals(exception),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _ok ? 1 : 0;

    public override string ToString() => _ok ? "Ok()" : $"Error({_exception?.GetType().NameOf()}): {_exception?.Message})";
}