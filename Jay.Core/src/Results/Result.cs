namespace Jay;

/// <summary>
/// <b>Result</b><br />
/// Represents the result of an <c>action</c> as either of:<br />
/// <see cref="Ok" /> - A completed operation<br />
/// <see cref="Error" /> - A failed operation with an <see cref="Exception" />
/// </summary>
/// <remarks>
/// Heavily inspired by Rust's Result
/// </remarks>
public readonly partial struct Result :
    IEquatable<Result>,
    IEquatable<bool>,
    IEquatable<Exception>
{
    // default(Result) == (default(bool), default(Exception)
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
        return _error ?? new Exception(DEFAULT_ERROR_MESSAGE);
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

    public TReturn ThrowIfError<TReturn>(TReturn returnValue)
    {
        return _ok ? returnValue : throw GetError();
    }
    
    public Span<T> ThrowIfError<T>(Span<T> returnSpan)
    {
        return _ok ? returnSpan : throw GetError();
    }
    
    public ReadOnlySpan<T> ThrowIfError<T>(ReadOnlySpan<T> returnSpan)
    {
        return _ok ? returnSpan : throw GetError();
    }

    /// <summary>
    /// Is this an <see cref="Ok"/> <see cref="Result"/>?
    /// </summary>
    /// <returns></returns>
    public bool IsOk()
    {
        return _ok;
    }

    /// <summary>
    /// Is this an <see cref="Error"/> <see cref="Result"/>?
    /// </summary>
    public bool IsError()
    {
        return !_ok;
    }

    /// <summary>
    /// Is this a failed <see cref="Result" />?
    /// </summary>
    /// <param name="error">
    /// If this is <see cref="Error"/>, the attached <see cref="Exception"/>;<br/>
    /// otherwise <see langword="null" /></param>
    /// <returns><c>true</c> if this is an <see cref="Error"/>; otherwise, <c>false</c></returns>
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

    public TReturn Match<TReturn>(Func<TReturn> ok, Func<Exception, TReturn> error)
    {
        if (_ok)
        {
            return ok();
        }
        return error(GetError());
    }
    
    public bool Equals(Result result)
    {
        return result._ok == _ok;
    }
    
    public bool Equals(bool ok)
    {
        return ok == _ok;
    }
    
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
            () => nameof(Ok),
            error => $"Error({error.GetType().Name}): {error.Message}");
    }
}