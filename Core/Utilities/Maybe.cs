namespace Jay;

/// <summary>
/// Represents an option/maybe-typed value.
/// </summary>
/// <typeparam name="T">The type of value that can be stored.</typeparam>
/// <see cref="https://en.wikipedia.org/wiki/Option_type"/>
public readonly struct Maybe<T> : IEnumerable<T>
{
    /// <summary>
    /// Implicitly converts a <paramref name="value"/> to an <see cref="Maybe{T}"/> containing that value.
    /// </summary>
    /// <param name="value">
    /// The value to put in the <see cref="Maybe{T}"/>.
    /// If <see langword="null"/>, <see cref="None"/>; otherwise <see cref="Some"/>.
    /// </param>
    public static implicit operator Maybe<T>(T? value)
    {
        if (value is null) return None;
        return Some(value);
    }

    public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);
    public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);
    public static bool operator ==(Maybe<T> left, T right) => left.Equals(right);
    public static bool operator !=(Maybe<T> left, T right) => !left.Equals(right);
    
    /// <summary>
    /// Represents an <see cref="Maybe{T}"/> that does not contain a value.
    /// </summary>
    public static Maybe<T> None = new Maybe<T>();

    public static Maybe<T> Some(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Maybe<T>(value);
    }

    /// <summary>
    /// Does this <see cref="Maybe{T}"/> have a value?
    /// </summary>
    public readonly bool HasValue;
    
    /// <summary>
    /// The actual value stored in this <see cref="Maybe{T}"/>
    /// </summary>
    internal readonly T? _value;
       
    /// <summary>
    /// Construct a new <see cref="Maybe{T}"/> containing the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    private Maybe(T? value)
    {
        _value = value;
        HasValue = value is not null;
    }

    /// <summary>
    /// Attempts to get the stored <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>True if this <see cref="Maybe{T}"/> contained a value; otherwise, false.</returns>
    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return HasValue;
    }

    /// <summary>
    /// Gets the contained value or the default for the value type.
    /// </summary>
    /// <returns></returns>
    public T? ValueOrDefault() => HasValue ? _value : default!;

    /// <summary>
    /// Gets the contained value or the specified <paramref name="default"/> value.
    /// </summary>
    /// <returns></returns>
    public T? ValueOrDefault(T? @default) => HasValue ? _value : @default;

    public IEnumerator<T> GetEnumerator()
    {
        if (HasValue)
            yield return _value!;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(Maybe<T> maybe)
    {
        if (HasValue != maybe.HasValue) return false;
        if (!HasValue) return true;
        return EqualityComparer<T>.Default.Equals(maybe._value, _value);
    }

    public bool Equals(T value)
    {
        return HasValue && EqualityComparer<T>.Default.Equals(value, _value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Maybe<T> option) return Equals(option);
        if (obj is T value) return Equals(value);
        if (obj is null) return !HasValue;
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (!HasValue) return 0;
        return _value!.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (!HasValue)
            return nameof(None);
        return _value!.ToString() ?? string.Empty;
    }
}