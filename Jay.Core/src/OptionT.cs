using System.Collections;
using Jay.Utilities;

namespace Jay;

/// <summary>
/// An <c>Option</c> represents either:<br/>
/// <see cref="Some"/> <typeparamref name="T"/> value<br/>
/// or<br/>
/// <see cref="None"/> (no value)
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Option<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<T>,
    IEnumerable<T>
{
    public static bool operator ==(Option<T> option, Option<T> right) => option.Equals(right);
    public static bool operator !=(Option<T> option, Option<T> right) => !option.Equals(right);
    public static bool operator ==(Option<T> option, T? value) => option.Equals(value);
    public static bool operator !=(Option<T> option, T? value) => !option.Equals(value);
    public static bool operator ==(T? value, Option<T> option) => option.Equals(value);
    public static bool operator !=(T? value, Option<T> option) => !option.Equals(value);
    public static bool operator ==(Option<T> option, object? obj) => option.Equals(obj);
    public static bool operator !=(Option<T> option, object? obj) => !option.Equals(obj);
    public static bool operator ==(object? obj, Option<T> option) => option.Equals(obj);
    public static bool operator !=(object? obj, Option<T> option) => !option.Equals(obj);

    /// <summary>
    /// Create an <see cref="Option{T}"/> containing <paramref name="value"/>
    /// </summary>
    public static Option<T> Some(T value) => new(true, value);

    /// <summary>
    /// Represents no value, the same as <see cref="Void"/>
    /// </summary>
    public static readonly Option<T> None = default;
    
    
    private readonly bool _some;
    private readonly T _value;

    private Option(bool some, T value)
    {
        _some = some;
        _value = value;
    }

    public bool IsSome() => _some;

    public bool IsSome(out T value)
    {
        value = _value;
        return _some;
    }

    public bool IsNone() => !_some;

    public void Match(Action<T> some, Action none)
    {
        if (_some)
        {
            some(_value);
        }
        else
        {
            none();
        }
    }

    public void Match(Action<T> some, Action<Nothing> none)
    {
        if (_some)
        {
            some(_value);
        }
        else
        {
            none(default);
        }
    }

    public TReturn Match<TReturn>(Func<T, TReturn> some, Func<TReturn> none)
    {
        if (_some)
        {
            return some(_value);
        }
        return none();
    }

    public TReturn Match<TReturn>(Func<T, TReturn> some, Func<Nothing, TReturn> none)
    {
        if (_some)
        {
            return some(_value);
        }
        return none(default);
    }

    public Result<T> AsResult(Exception? error = null)
    {
        if (_some)
        {
            return Result<T>.Ok(_value);
        }
        return Result<T>.Error(error);
    }

    public Result<T, E> AsResult<E>(Func<E> errorIfNone)
        where E : Exception
    {
        if (_some)
        {
            return Result<T, E>.Ok(_value);
        }
        return Result<T, E>.Error(errorIfNone());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        if (_some)
        {
            yield return _value;
        }
    }

    public bool Equals(Option<T> option)
    {
        if (_some)
        {
            return option.IsSome(out var value) && EqualityComparer<T>.Default.Equals(_value, value);
        }
        return !option._some;
    }

    public bool Equals(T? value)
    {
        return _some && EqualityComparer<T?>.Default.Equals(_value, value);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return _some ? Hasher.GetHashCode(_value) : 0;
    }

    public override string ToString()
    {
        return _some ? $"Some({_value})" : "None";
    }
}