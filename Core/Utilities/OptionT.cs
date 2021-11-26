using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    public readonly struct Option<T> : IEquatable<Option<T>>,
                                       IEquatable<T>,
                                       IEnumerable<T>, IEnumerable
    {
        public static implicit operator Option<T>(T value) => new Option<T>(value);

        public static bool operator ==(Option<T> x, Option<T> y) => x.Equals(y);
        public static bool operator !=(Option<T> x, Option<T> y) => !x.Equals(y);
        public static bool operator ==(Option<T> x, T? value) => x.Equals(value);
        public static bool operator !=(Option<T> x, T? value) => !x.Equals(value);

        public static T? operator |(Option<T> option, T? value)
        {
            if (option.TryGetValue(out var optionValue))
                return optionValue;
            return value;
        }

        public static readonly Option<T> None = default;

        private readonly bool _hasValue;
        private readonly T? _value;

        public bool HasValue => _hasValue;

        public Option(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public bool TryGetValue([NotNullWhen(true)] out T? value)
        {
            if (_hasValue)
            {
                value = _value!;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            if (_hasValue)
            {
                yield return _value!;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Option<T> option)
        {
            if (_hasValue)
            {
                if (option._hasValue)
                {
                    return EqualityComparer<T>.Default.Equals(_value, option._value);
                }

                return false;
            }
            return option._hasValue == false;
        }

        public bool Equals(T? value)
        {
            if (value is null)
                return !_hasValue;
            return _hasValue && EqualityComparer<T>.Default.Equals(_value, value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Option<T> option) return Equals(option);
            if (obj is T value) return Equals(value);
            if (obj is null) return !_hasValue;
            return false;
        }

        public override int GetHashCode()
        {
            if (_hasValue)
                return _value!.GetHashCode();
            return 0;
        }

        public override string ToString()
        {
            if (_hasValue)
                return _value!.ToString() ?? "";
            return nameof(None);
        }
    }
}
