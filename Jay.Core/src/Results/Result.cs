using Jay.Reflection;

namespace Jay;

/// <summary>
/// Represents the result of an operation as either:<br/>
/// <c>Ok</c><br/>
/// <c>Error(<see cref="Exception"/>)</c>
/// </summary>
public readonly partial struct Result :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result, Result, bool>,
    IEqualityOperators<Result, bool, bool>,
    IBitwiseOperators<Result, Result, bool>,
    IBitwiseOperators<Result, bool, bool>,
#endif
    IEquatable<Result>
{
    public static implicit operator Result(bool isOk) => isOk ? Ok() : Error(null);
    public static implicit operator Result(Exception? error) => Error(error);
    public static implicit operator bool(Result result) => result._ok;

    public static bool operator true(Result result) => result._ok;
    public static bool operator false(Result result) => !result._ok;
    public static bool operator !(Result result) => !result._ok;

    public static bool operator ~(Result _)
    {
        throw new NotSupportedException("Cannot apply ~ to a Result");
    }

    public static bool operator ==(Result left, Result right) => left.Equals(right);
    public static bool operator ==(Result result, Exception? error) => result.Equals(error);
    public static bool operator ==(Result result, bool pass) => result.Equals(pass);
    public static bool operator ==(Exception? error, Result result) => result.Equals(error);
    public static bool operator ==(bool pass, Result result) => result.Equals(pass);

    public static bool operator !=(Result left, Result right) => !left.Equals(right);
    public static bool operator !=(Result result, Exception? error) => !result.Equals(error);
    public static bool operator !=(Result result, bool pass) => !result.Equals(pass);
    public static bool operator !=(Exception? error, Result result) => !result.Equals(error);
    public static bool operator !=(bool pass, Result result) => !result.Equals(pass);

    public static bool operator |(Result left, Result right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result result, bool pass) => pass || result.IsOk();
    public static bool operator |(bool pass, Result result) => pass || result.IsOk();

    public static bool operator &(Result left, Result right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result result, bool pass) => pass && result.IsOk();
    public static bool operator &(bool pass, Result result) => pass && result.IsOk();

    public static bool operator ^(Result left, Result right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result result, bool pass) => pass ^ result.IsOk();
    public static bool operator ^(bool pass, Result result) => pass ^ result.IsOk();

    /// <summary>
    /// Returns a passing <see cref="Result" />
    /// </summary>
    public static Result Ok()
    {
        return new(true, null);
    }

    /// <summary>
    /// Returns a failing <see cref="Result" /> with the given <paramref name="error" />.
    /// </summary>
    /// <param name="error">The failing <see cref="_exception" />.</param>
    /// <returns>A failed <see cref="Result" />.</returns>
    public static Result Error(Exception? error)
    {
        return new(false, error);
    }


    private readonly bool _ok;
    private readonly Exception? _exception;

    internal Result(bool ok, Exception? exception)
    {
        _ok = ok;
        _exception = exception;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception GetException()
    {
        return _exception ?? new Exception();
    }
    
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
            throw GetException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _ok;

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

        error = GetException();
        return true;
    }

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
    
    public bool Equals(Result result) => _ok == result.IsOk();
    public bool Equals(Exception? _) => !_ok;
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result result => Equals(result),
            Exception ex => Equals(ex),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _ok ? 1 : 0;

    public override string ToString()
    {
        return Match(
            () => "Ok()",
            error => $"Error({error.GetType().ToCode()}): {error.Message})");
    }
}