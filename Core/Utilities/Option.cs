using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    public readonly struct Option<T> : IEnumerable<T>
    {
        /// <summary>
        /// Implicitly converts a <paramref name="value"/> to an <see cref="Option{T}"/> containing that value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Option<T>(T value) => Some(value);

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
        public static bool operator ==(Option<T> left, T right) => left.Equals(right);
        public static bool operator !=(Option<T> left, T right) => !left.Equals(right);

        /// <summary>
        /// Represents an <see cref="Option{T}"/> that does not contain a value.
        /// </summary>
        public static Option<T> None = new Option<T>();

        public static Option<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Option<T>(value);
        }

        private readonly bool _hasValue;
        private readonly T? _value;

        /// <summary>
        /// Does this <see cref="Option{T}"/> have a value?
        /// </summary>
        public bool HasValue => _hasValue;
       
        /// <summary>
        /// Construct a new <see cref="Option{T}"/> containing the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        private Option(T? value)
        {
            _value = value;
            _hasValue = value is not null;
        }

        /// <summary>
        /// Attempts to get the stored <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if this <see cref="Option{T}"/> contained a value; otherwise, false.</returns>
        public bool TryGetValue([NotNullWhen(true)] out T? value)
        {
            value = _value;
            return _hasValue;
        }

        /// <summary>
        /// Gets the contained value or the default for the value type.
        /// </summary>
        /// <returns></returns>
        public T? ValueOrDefault() => _hasValue ? _value : default!;

        /// <summary>
        /// Gets the contained value or the specified <paramref name="default"/> value.
        /// </summary>
        /// <returns></returns>
        public T? ValueOrDefault(T? @default) => _hasValue ? _value : @default;

        public IEnumerator<T> GetEnumerator()
        {
            if (_hasValue)
                yield return _value!;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Option<T> option)
        {
            return _hasValue && 
                   option._hasValue &&
                   EqualityComparer<T>.Default.Equals(option._value, _value);
        }

        public bool Equals(T value)
        {
            return _hasValue && EqualityComparer<T>.Default.Equals(value, _value);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Option<T> option) return Equals(option);
            if (obj is T value) return Equals(value);
            return false;
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
}
