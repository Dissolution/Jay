using System.Diagnostics;

namespace Jay.Utilities;

/// <summary>
/// Helpers related to <see cref="Option{T}"/>
/// </summary>
public readonly struct Option
{
    /// <summary>
    /// A universal <c>Option.None</c> that maps to any <c>Option&lt;T&gt;.None</c>
    /// </summary>
    public static readonly Option None = default;

    /// <summary>
    /// Creates a <c>Option&lt;T&gt;.Some</c> containing the non-<c>null</c> <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/></typeparam>
    /// <param name="value">The value to store as a Some</param>
    /// <returns>An <see cref="Option{T}"/> that contains <paramref name="value"/></returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is <c>null</c>
    /// </exception>
    public static Option<T> Some<T>([NotNull] T value) => Option<T>.Some(value);

    public static Option<T> Create<T>(T? value) => value is null ? Option<T>.None : new Option<T>(value);
}
public readonly struct Option<T> :
#if NET7_0_OR_GREATER
IEqualityOperators<Option<T>, Option<T>, bool>,    
IEqualityOperators<Option<T>, T, bool>,
IEqualityOperators<Option<T>, Option, bool>,
#endif
    IEquatable<Option<T>>, IEquatable<T>,
    IEnumerable<T>
{
    public static implicit operator Option<T>(T? value) => value is null ? None : new Option<T>(value);
    public static implicit operator Option<T>(Option none) => None;

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator ==(Option<T> option, T? value) => option.Equals(value);
    public static bool operator !=(Option<T> option, T? value) => !option.Equals(value);
    public static bool operator ==(Option<T> option, Option none) => !option._hasValue;
    public static bool operator !=(Option<T> option, Option none) => option._hasValue;

    public static readonly Option<T> None = default;

    public static Option<T> Some([NotNull] T value)
    {
        Validate.NotNull(value);
        return new Option<T>(value);
    }

    private readonly T? _value;
    private readonly bool _hasValue;

    public bool IsNone => !_hasValue;
    
    internal Option([NotNull] T value)
    {
        Debug.Assert(value is not null);
        _value = value;
        _hasValue = true;
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return _hasValue;
    }

    public T? ValueOrDefault() => _value;
    
    public T? ValueOrDefault(T? @default) => _hasValue ? _value : @default;
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        if (_hasValue)
            yield return _value!;
    }

    public bool Equals(Option<T> option)
    {
        if (_hasValue != option._hasValue) return false;
        if (!_hasValue) return true;
        return EqualityComparer<T>.Default.Equals(_value!, option._value!);
    }
    
    public bool Equals(T? value)
    {
        if (value is null) return !_hasValue;
        return EqualityComparer<T>.Default.Equals(_value!, value);
    }
    
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            null => !_hasValue,
            _ => false,
        };
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (!_hasValue) return 0;
        return _value!.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (!_hasValue)
            return nameof(None);
        return _value!.ToString() ?? string.Empty;
    }
}