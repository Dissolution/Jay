using System.Collections;
using Jay.Reflection;
using Jay.Utilities;

namespace Jay;

/// <summary>
/// Represents the typed result of an operation as either:<br/>
/// <c>Ok(<typeparamref name="V">Value</typeparamref>)</c><br/>
/// <c>Error(<typeparamref name="E">Error</typeparamref>)</c>
/// </summary>
/// <typeparam name="V">
/// The <see cref="Type"/> of the Value stored with an <c>Ok</c> Result
/// </typeparam>
/// <typeparam name="E">
/// The <see cref="Type"/> of the Value stored with an <c>Error</c> Result 
/// </typeparam>
public readonly struct Result<V, E> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<V, E>, Result<V, E>, bool>,
    IEqualityOperators<Result<V, E>, bool, bool>,
    IBitwiseOperators<Result<V, E>, Result<V, E>, bool>,
    IBitwiseOperators<Result<V, E>, bool, bool>,
#endif
    IEquatable<Result<V, E>>,
    IEquatable<bool>,
    IEnumerable<V>
{
    public static implicit operator Result<V, E>(V value) => Ok(value);
    public static implicit operator Result<V, E>(E error) => Error(error);
    public static implicit operator bool(Result<V, E> result) => result._ok;

    public static bool operator true(Result<V, E> result) => result._ok;
    public static bool operator false(Result<V, E> result) => !result._ok;
    public static bool operator !(Result<V, E> result) => !result._ok;
    public static bool operator ~(Result<V, E> _) 
        => throw new NotSupportedException($"Cannot apply ~ to a Result<{TypeNames.ToCode<V>()},{TypeNames.ToCode<E>()}>");

    public static bool operator ==(Result<V, E> left, Result<V, E> right) => left.Equals(right);
    public static bool operator ==(Result<V, E> result, V value) => result.Equals(value);
    public static bool operator ==(Result<V, E> result, E error) => result.Equals(error);
    public static bool operator ==(Result<V, E> result, bool pass) => result.Equals(pass);
    public static bool operator ==(V value, Result<V, E> result) => result.Equals(value);
    public static bool operator ==(E error, Result<V, E> result) => result.Equals(error);
    public static bool operator ==(bool pass, Result<V, E> result) => result.Equals(pass);

    public static bool operator !=(Result<V, E> left, Result<V, E> right) => !left.Equals(right);
    public static bool operator !=(Result<V, E> result, V value) => !result.Equals(value);
    public static bool operator !=(Result<V, E> result, E error) => !result.Equals(error);
    public static bool operator !=(Result<V, E> result, bool pass) => !result.Equals(pass);
    public static bool operator !=(V value, Result<V, E> result) => !result.Equals(value);
    public static bool operator !=(E error, Result<V, E> result) => !result.Equals(error);
    public static bool operator !=(bool pass, Result<V, E> result) => !result.Equals(pass);

    public static bool operator |(Result<V, E> left, Result<V, E> right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result<V, E> result, bool pass) => pass || result.IsOk();
    public static bool operator |(bool pass, Result<V, E> result) => pass || result.IsOk();
    
    public static bool operator &(Result<V, E> left, Result<V, E> right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result<V, E> result, bool pass) => pass && result.IsOk();
    public static bool operator &(bool pass, Result<V, E> result) => pass && result.IsOk();

    public static bool operator ^(Result<V, E> left, Result<V, E> right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result<V, E> result, bool pass) => pass ^ result.IsOk();
    public static bool operator ^(bool pass, Result<V, E> result) => pass ^ result.IsOk();

    /// <summary>
    /// Get an <c>Ok</c> <see cref="Result{V,E}"/> with attached <paramref name="okValue"/>
    /// </summary>
    public static Result<V, E> Ok(V okValue)
    {
        return new(true, okValue, default);
    }
    
    /// <summary>
    /// Gets an <c>Error</c> <see cref="Result{V,E}"/> with attached <paramref name="errorValue"/>
    /// </summary>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static Result<V, E> Error(E? errorValue)
    {
        return new(false, default!, errorValue);
    }
    
    
    private readonly bool _ok;
    private readonly V _value;
    private readonly E? _error;

    internal Result(bool ok, V value, E? error)
    {
        _ok = ok;
        _value = value;
        _error = error;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _ok;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out V value)
    {
        value = _value;
        return _ok;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_ok;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError(out E? error)
    {
        error = _error;
        return !_ok;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<V> onOk, Action<E?> onError)
    {
        if (_ok)
        {
            onOk(_value);
        }
        else
        {
            onError(_error);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<V, TReturn> onOk, Func<E?, TReturn> onError)
    {
        if (_ok)
        {
            return onOk(_value);
        }
        else
        {
            return onError(_error);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<V> AsOption()
    {
        return _ok ? Option<V>.Some(_value) : Option<V>.None;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<V> GetEnumerator()
    {
        if (_ok)
        {
            yield return _value;
        }
    }
    
    public bool Equals(Result<V, E> result)
    {
        if (_ok)
        {
            return result.IsOk(out var okValue) && EqualityComparer<V>.Default.Equals(_value!, okValue!);
        }
        else
        {
            return result.IsError(out var errorValue) && EqualityComparer<E>.Default.Equals(_error!, errorValue!);
        }
    }
  
    public bool Equals(V? okValue) => _ok && EqualityComparer<V>.Default.Equals(_value!, okValue!);
    public bool Equals(E? errorValue) => !_ok && EqualityComparer<E>.Default.Equals(_error!, errorValue!);
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<V, E> fullResult => Equals(fullResult),
            V value => Equals(value),
            E ex => Equals(ex),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _ok ? Hasher.GetHashCode<V>(_value) : Hasher.GetHashCode<E>(_error);

    public override string ToString()
    {
        return Match(
            ok => $"Ok({TypeNames.ToCode<V>()}: {ok})",
            error => $"Error({TypeNames.ToCode<E>()}: {error})");
    }
}