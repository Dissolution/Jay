namespace Jay;

/// <summary>
/// <b>Result</b><br />
/// Represents the result of an <c>action</c> as either of:<br />
/// <see cref="Ok" /> - A completed operation<br />
/// <see cref="Error" /> - A failed operation with an <see cref="Exception" />
/// </summary>
public readonly partial struct Result :
    IEquatable<Result>,
    IEquatable<bool>,
    IEquatable<Exception>
{
    /* This is heavily inspired by Rust's Result discriminated union
     *
     * The user should never use default(Result), but if they do, we want to treat it as a failure.
     * As default(bool) == false
     * This means we do not get an Exception, so we'll try to capture one as soon as we can
     */

    internal readonly bool _ok;
    internal readonly Exception? _error;

    /// <remarks>
    /// We never let a consumer create a Result directly. They must use the implicit casts or the methods.
    /// </remarks>
    internal Result(bool ok, Exception? error)
    {
        _ok = ok;
        _error = error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception GetError()
    {
        return _error ?? new Exception(DefaultErrorMessage);
    }

    /// <summary>
    /// Throws the attached <see cref="Exception" /> if this is a failed <see cref="Result" />
    /// </summary>
    /// <exception cref="Exception">The attached exception.</exception>
    public void ThrowIfError()
    {
        if (!_ok)
        {
            throw GetError();
        }
    }

    public bool IsOk()
    {
        return _ok;
    }

    /// <summary>
    /// Is this a failed <see cref="Result" />?
    /// </summary>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    public bool IsError()
    {
        return !_ok;
    }

    /// <summary>
    /// Is this a failed <see cref="Result" />?
    /// </summary>
    /// <param name="error">If this is a failed <see cref="Result" />, the attached (or a new) <see cref="Exception" />; otherwise <see langword="null" /></param>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    public bool IsError([NotNullWhen(true)] out Exception? error)
    {
        if (_ok)
        {
            error = null;
            return false;
        }

        error = GetError();
        return true;
    }

    public void Match(Action ok, Action<Exception> error)
    {
        if (_ok)
        {
            ok();
        }
        else
        {
            error(GetError());
        }
    }

    public void Match(Action<None> ok, Action<Exception> error)
    {
        if (_ok)
        {
            ok(default);
        }
        else
        {
            error(GetError());
        }
    }

    public TReturn Match<TReturn>(Func<TReturn> ok, Func<Exception, TReturn> error)
    {
        if (_ok)
        {
            return ok();
        }
        return error(GetError());
    }

    public TReturn Match<TReturn>(Func<None, TReturn> ok, Func<Exception, TReturn> error)
    {
        if (_ok)
        {
            return ok(default);
        }
        return error(GetError());
    }

    /// <inheritdoc cref="IEquatable{T}" />
    public bool Equals(Result result)
    {
        return result._ok == _ok;
    }

    /// <inheritdoc cref="IEquatable{T}" />
    public bool Equals(bool ok)
    {
        return ok == _ok;
    }

    /// <inheritdoc cref="IEquatable{T}" />
    public bool Equals(Exception? _)
    {
        return !_ok;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result result => Equals(result),
            bool ok => Equals(ok),
            Exception ex => Equals(ex),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return _ok ? 1 : 0;
    }

    public override string ToString()
    {
        return Match(
            _ => nameof(Ok),
            error => $"Error({error.GetType().Name}): {error.Message}");
    }
}