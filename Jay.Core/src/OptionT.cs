using Jay.Utilities;

namespace Jay;

/// <summary>
/// An <c>Option</c> represents either:<br/>
/// <see cref="Some">Some(T value)</see><br/>
/// or<br/>
/// <see cref="None"/>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value stored with <see cref="Some"/>
/// </typeparam>
public readonly struct Option<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<T>,
    IEnumerable<T>
{
    public static bool operator ==(Option<T> option, Option<T> otherOption) => option.Equals(otherOption);
    public static bool operator !=(Option<T> option, Option<T> otherOption) => !option.Equals(otherOption);
    public static bool operator ==(Option<T> option, T? value) => option.Equals(value);
    public static bool operator !=(Option<T> option, T? value) => !option.Equals(value);
    public static bool operator ==(T? value, Option<T> option) => option.Equals(value);
    public static bool operator !=(T? value, Option<T> option) => !option.Equals(value);
    public static bool operator ==(Option<T> option, object? obj) => option.Equals(obj);
    public static bool operator !=(Option<T> option, object? obj) => !option.Equals(obj);
    public static bool operator ==(object? obj, Option<T> option) => option.Equals(obj);
    public static bool operator !=(object? obj, Option<T> option) => !option.Equals(obj);

    /// <summary>
    /// Creates an <see cref="Option{T}"/>.<see cref="Some"/> containing the given <paramref name="value"/>
    /// </summary>
    public static Option<T> Some(T value) => new(true, value);

    /// <summary>
    /// Gets <see cref="Option{T}"/>.<see cref="None"/>
    /// </summary>
    public static readonly Option<T> None = default;
    
    
    private readonly bool _some;
    private readonly T _value;

    private Option(bool some, T value)
    {
        _some = some;
        _value = value;
    }

    /// <summary>
    /// Is this <see cref="Option{T}"/> a <see cref="Some"/>?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is <see cref="Some"/>, <c>false</c> if this is <see cref="None"/>
    /// </returns>
    public bool IsSome() => _some;

    /// <summary>
    /// Is this <see cref="Option{T}"/> a <see cref="Some"/>, and if so, get the <typeparamref name="T"/> <paramref name="value"/>
    /// </summary>
    /// <param name="value">
    /// If this is <see cref="Some"/>, the <typeparamref name="T"/> value, otherwise <c>default(T)</c>
    /// </param>
    /// <returns>
    /// <c>true</c> if this is <see cref="Some"/>, <c>false</c> if this is <see cref="None"/>
    /// </returns>
    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _some;
    }

    /// <summary>
    /// Is this <see cref="Option{T}"/> a <see cref="None"/>?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is <see cref="None"/>, <c>false</c> if this is <see cref="Some"/>
    /// </returns>
    public bool IsNone() => !_some;

    /// <summary>
    /// Perform an action depending on if this <see cref="Option{T}"/> is:<br/>
    /// <see cref="Some"/> =&gt; <paramref name="some"/><br/>
    /// <see cref="None"/> =&gt; <paramref name="none"/>
    /// </summary>
    /// <param name="some">
    /// The <see cref="Action{T}"/> to invoke if this is <see cref="Some"/>, passing the <typeparamref name="T"/> value
    /// </param>
    /// <param name="none">
    /// The <see cref="Action"/> to invoke if this is <see cref="None"/>
    /// </param>
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

    /// <summary>
    /// Perform an action depending on if this <see cref="Option{T}"/> is:<br/>
    /// <see cref="Some"/> =&gt; <paramref name="some"/><br/>
    /// <see cref="None"/> =&gt; <paramref name="none"/>
    /// </summary>
    /// <param name="some">
    /// The <see cref="Action{T}"/> to invoke if this is <see cref="Some"/>, passing the <typeparamref name="T"/> value
    /// </param>
    /// <param name="none">
    /// The <see cref="Action">Action&lt;Nothing&gt;</see> to invoke if this is <see cref="None"/>
    /// </param>
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

    /// <summary>
    /// Perform a <see cref="Func{TResult}"/> depending on if this <see cref="Option{T}"/> is:<br/>
    /// <see cref="Some"/> =&gt; <paramref name="some"/><br/>
    /// <see cref="None"/> =&gt; <paramref name="none"/>
    /// </summary>
    /// <param name="some">
    /// The <see cref="Func{T,TResult}"/> to invoke if this is <see cref="Some"/>, passing the <typeparamref name="T"/> value
    /// </param>
    /// <param name="none">
    /// The <see cref="Func{TResult}"/> to invoke if this is <see cref="None"/>
    /// </param>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        if (_some)
        {
            return some(_value);
        }
        else
        {
            return none();
        }
    }

    /// <summary>
    /// Perform a <see cref="Func{TResult}"/> depending on if this <see cref="Option{T}"/> is:<br/>
    /// <see cref="Some"/> =&gt; <paramref name="some"/><br/>
    /// <see cref="None"/> =&gt; <paramref name="none"/>
    /// </summary>
    /// <param name="some">
    /// The <see cref="Func{T, TResult}"/> to invoke if this is <see cref="Some"/>, passing the <typeparamref name="T"/> value
    /// </param>
    /// <param name="none">
    /// The <see cref="Func{T, TResult}">Func&lt;Nothing,TResult&gt;</see> to invoke if this is <see cref="None"/>
    /// </param>
    public TReturn Match<TReturn>(Func<T, TReturn> some, Func<Nothing, TReturn> none)
    {
        if (_some)
        {
            return some(_value);
        }
        else
        {
            return none(default);
        }
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
            return option.IsSome(out var value) && 
                EqualityComparer<T>.Default.Equals(_value, value);
        }
        return !option._some;
    }

    public bool Equals(T? value)
    {
        return _some && EqualityComparer<T>.Default.Equals(_value!, value!);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            null => _some && _value is null,
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