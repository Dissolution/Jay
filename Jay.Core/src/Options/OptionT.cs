using System.Collections;

namespace Jay;

public readonly partial struct Option<T> :
    IEquatable<Option<T>>,
    IEquatable<T>,
    //IEquatable<None>,
    IEnumerable<T>
{
    private readonly bool _some;
    private readonly T? _value;

    private Option(bool some, T? value)
    {
        _some = some;
        _value = value;
    }

    public bool IsSome([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return _some;
    }

    public bool IsNone()
    {
        return !_some;
    }

    public void Match(Action<T> some, Action none)
    {
        if (_some)
        {
            some(_value!);
        }
        else
        {
            none();
        }
    }

    public void Match(Action<T> some, Action<None> none)
    {
        if (_some)
        {
            some(_value!);
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
            return some(_value!);
        }
        return none();
    }

    public TReturn Match<TReturn>(Func<T, TReturn> some, Func<None, TReturn> none)
    {
        if (_some)
        {
            return some(_value!);
        }
        return none(default);
    }

    public Result<T> AsResult(Exception? error = null)
    {
        if (_some)
        {
            return Result<T>.Ok(_value!);
        }
        return Result<T>.Error(error);
    }

    public Result<T, E> AsResult<E>(Func<E> errorIfNone)
        where E : Exception
    {
        if (_some)
        {
            return Result<T, E>.Ok(_value!);
        }
        return Result<T, E>.Error(errorIfNone());
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_some)
        {
            yield return _value!;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(Option<T> option)
    {
        if (_some)
        {
            return option._some && EqualityComparer<T>.Default.Equals(_value!, option._value!);
        }
        return !option._some;
    }

    public bool Equals(T? value)
    {
        if (value is not null)
        {
            return _some && EqualityComparer<T>.Default.Equals(_value!, value);
        }
        return !_some;
    }
    public bool Equals(None _)
    {
        return !_some;
    }

    public override bool Equals(object? obj)
    {
        if (obj.IsNone()) return !_some;
        if (obj is T value) return Equals(value);
        if (obj is Option<T> option) return Equals(option);
        return false;
    }

    public override int GetHashCode()
    {
        if (!_some) return 0;
        return _value!.GetHashCode();
    }

    public override string ToString()
    {
        return Match(
            some => $"Some({some})",
            () => nameof(None));
    }
}