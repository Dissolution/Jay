namespace Jay.Result;

/// <summary>
/// Represents the result of an operation as a Pass or a Failure with <see cref="Exception"/> information.
/// </summary>
/// <remarks>
/// non-<c>static</c> parts of <see cref="Result"/>
/// </remarks>
public readonly partial struct Result : IEquatable<Result>, IEquatable<bool>
{
    /* These fields were chosen specifically
     * I wanted `default(Result)` to be the same as `default(bool)` -- which is `false`
     * Hence, the default values of all fields should indicate failure
     */
    
    /// <summary>
    /// Does this <see cref="Result"/> indicate a Pass (<c>true</c>) or a Fail (<c>false</c>)?
    /// </summary>
    internal readonly bool _pass;
    
    /// <summary>
    /// If this is a Failed <see cref="Result"/>, this contains an optional attached <see cref="Exception"/> related to the failure
    /// </summary>
    internal readonly Exception? _error;

    /// <remarks>
    /// We never let a consumer create a Result directly. <br/>
    /// They must use the implicit casts or the methods.
    /// </remarks>
    internal Result(bool pass, Exception? error)
    {
        _pass = pass;
        _error = error;
        Debug.Assert((pass == true && error is null) || (pass == false && error is not null));
    }

    /// <summary>
    /// Throws the attached <see cref="Exception"/> if this is a failed <see cref="Result"/>
    /// </summary>
    /// <exception cref="Exception">The attached exception.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfFailed()
    {
        if (!_pass)
        {
            Debug.Assert(_error is not null);
            throw _error;
        }
    }

    /// <summary>
    /// Is this a failed <see cref="Result"/>?
    /// </summary>
    /// <returns><c>true</c> if this is a failed result; otherwise, <c>false</c></returns>
    public bool IsFailure()
    {
        return !_pass;
    }

    /// <summary>
    /// Is this a failed <see cref="Result"/>?
    /// </summary>
    /// <param name="error">If this is a failed <see cref="Result"/>, the attached (or a new) <see cref="Exception"/>; otherwise <c>null</c></param>
    /// <returns>true if this is a failed result; otherwise, false</returns>
    public bool IsFailure([NotNullWhen(true)] out Exception? error)
    {
        if (_pass)
        {
            error = null;
            return false;
        }

        Debug.Assert(_error is not null);
        error = _error;
        return true;
    }

    /// <summary>
    /// Returns a <see cref="Result{T}"/> that is this <see cref="Result"/> with an attached <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="type"/> of <paramref name="value"/> to attach to this <see cref="Result"/></typeparam>
    /// <param name="value">The <typeparamref name="T"/> value to attach to this <see cref="Result"/></param>
    /// <returns>A new <see cref="Result{T}"/></returns>
    public Result<T> WithValue<T>(T? value)
    {
        return new Result<T>(_pass, value, _error);
    }
    
    /// <summary>
    /// Does this <see cref="Result"/> indicate the same Pass/Fail as <paramref name="result"/>?
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to check for equality</param>
    /// <returns><c>true</c> if the results are both Pass or both Fail; otherwise, <c>false</c></returns>
    public bool Equals(Result result) => result._pass == this._pass;

    /// <summary>
    /// Does this <see cref="Result"/> indicate the same Pass/Fail as <paramref name="pass"/>?
    /// </summary>
    /// <param name="pass">The <see cref="bool"/> to check for equality</param>
    /// <returns><c>true</c> if this is a Pass result and pass is <c>true</c>; otherwise, <c>false</c></returns>
    public bool Equals(bool pass) => pass == this._pass;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result result => Equals(result),
            bool pass => Equals(pass),
            _ => false,
        };
    }
    
    public override int GetHashCode() => _pass ? 1 : 0;

    public override string ToString()
    {
        if (_pass)
            return bool.TrueString;
        Debug.Assert(_error is not null);
        return $"{_error.GetType().Name}: {_error.Message}";
    }
}