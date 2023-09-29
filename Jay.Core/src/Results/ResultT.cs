using System.Collections;
using Jay.Utilities;

namespace Jay;

/// <summary>
/// <b>Result&lt;T&gt;</b><br />
/// Represents the result of an <c>func&lt;T&gt;</c> as either of:<br />
/// <see cref="Ok" /> - A completed operation with value<br />
/// <see cref="Error" /> - A failed operation with an <see cref="Exception" />
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type" /> of value returned with <see cref="Ok" />
/// </typeparam>
public readonly partial struct Result<T> :
    IEquatable<Result<T>>,
    //IEquatable<Result>,
    //IEquatable<bool>,
    //IEquatable<Exception>,
    IEquatable<T>,
    IEnumerable<T>
{
    /* Just as Result, these fields were chosen for the same criteria. */
    internal readonly bool _ok;
    internal readonly T? _value;
    internal readonly Exception? _error;

    internal Result(bool ok, T? value, Exception? error)
    {
        _ok = ok;
        _value = value;
        _error = error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception GetError()
    {
        return _error ?? new Exception(Result.DEFAULT_ERROR_MESSAGE);
    }

    public T ValueOrThrow()
    {
        if (!_ok)
        {
            throw GetError();
        }
        return _value!;
    }

    /// <summary>
    /// Throws the attached <see cref="Exception" /> if this is a failed <see cref="Result{T}" />
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

    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
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
    /// <param name="error">If this is a failed <see cref="Result" />, the attached <see cref="Exception" />; otherwise <see langword="null" /></param>
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

    public void Match(Action<T> ok, Action<Exception> error)
    {
        if (_ok)
        {
            ok(_value!);
        }
        else
        {
            error(GetError());
        }
    }

    public TReturn Match<TReturn>(Func<T, TReturn> ok, Func<Exception, TReturn> error)
    {
        if (_ok)
        {
            return ok(_value!);
        }
        return error(GetError());
    }

    public Option<T> AsOption()
    {
        if (_ok)
        {
            return Option<T>.Some(_value!);
        }
        return Option<T>.None;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_ok)
        {
            yield return _value!;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    /// <inheritdoc cref="IEquatable{T}" />
    public bool Equals(Result<T> result)
    {
        if (_ok)
        {
            return result._ok && EqualityComparer<T>.Default.Equals(_value!, result._value!);
        }
        return result._ok == false;
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
    public bool Equals(T? value)
    {
        return _ok && EqualityComparer<T>.Default.Equals(_value!, value!);
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
            Result<T> resultT => Equals(resultT),
            Result result => Equals(result),
            bool pass => Equals(pass),
            T value => Equals(value),
            Exception ex => Equals(ex),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        if (!_ok) return 0;
        return Hasher.Combine(_value);
    }

    public override string ToString()
    {
        return Match(
            ok => $"Ok({ok})",
            error => $"Error({error.GetType().Name}): {error.Message}");
    }
}