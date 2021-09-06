using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Jay.Debugging.Dumping;
using Jay.Exceptions;
using Jay.Text;

namespace Jay
{
    public static class Box
    {
        public static Box<T> Up<T>([AllowNull] T value) => new Box<T>(value);

        public static Box<object?> Null => new Box<object?>();
        
        public static bool TryGetValue<T>(this Box<object> box, out T? value)
        {
            if (box.TryGetValue(out var obj))
            {
                return obj.Is(out value);
            }
            value = default;
            return false;
        }
    }
    
    public sealed class Box<T> : IEquatable<Box<T>>,
                                 IEquatable<T>,
                                 IEnumerable<T?>
    {
        public static implicit operator Box<T>([AllowNull] T value) => new Box<T>(value);
        
        public static Box<T> Empty => new Box<T>();

        public static Box<T> Up([AllowNull] T value)
        {
            return new Box<T>(true, value);
        }
        
        private bool _hasValue;
        private T? _value;

        public bool HasValue => _hasValue;

        public Type ValueType
        {
            get
            {
                if (_hasValue)
                {
                    return _value?.GetType() ?? typeof(T);
                }
                return typeof(T);
            }
        }

        public T? Value
        {
            get => _value;
            set
            {
                _value = value;
                _hasValue = true;
            }
        }
        
        private Box(bool hasValue, T? value)
        {
            _hasValue = hasValue;
            _value = value;
        }
        
        public Box() : this(false, default(T)) { }

        public Box([AllowNull] T value) : this(true, value) { }
    
        public bool TryGetValue([MaybeNull] out T value)
        {
            value = _value;
            return _hasValue;
        }
      
        public void SetValue([AllowNull] T value)
        {
            _value = value;
            _hasValue = true;
        }

        [return: MaybeNull]
        public T GetOrAdd([AllowNull] T value)
        {
            if (!_hasValue)
            {
                _value = value;
                _hasValue = true;
            }
            return _value;
        }
        
        public void Clear()
        {
            _hasValue = false;
            _value = default(T);
        }

        /// <inheritdoc />
        public bool Equals(Box<T>? box)
        {
            if (box is null) return !_hasValue;
            if (box._hasValue && _hasValue)
            {
                return EqualityComparer<T>.Default.Equals(box._value, _value);
            }
            return false;
        }
        
        public bool Equals([AllowNull] T value)
        {
            if (_hasValue)
                return EqualityComparer<T>.Default.Equals(value, _value);
            return false;
        }

        /// <inheritdoc />
        public IEnumerator<T?> GetEnumerator()
        {
            if (_hasValue)
                yield return _value;
        }
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return !_hasValue || _value is null;
            if (obj is Box<T> boxT)
                return Equals(boxT);
            if (_hasValue && obj.Is<T>(out T? objValue))
                return EqualityComparer<T>.Default.Equals(_value, objValue);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCodeException.Throw(this);

        /// <inheritdoc />
        public override string ToString()
        {
            if (_hasValue)
            {
                if (_value is not null)
                {
                    return _value.ToString() ?? string.Empty;
                }
                else
                {
                    return TextBuilder.Build(this, (tb, box) => tb.Append("default(").AppendDump(box.ValueType).Append(')'));
                }
            }
            else
            {
                return "Empty";
            }
        }
    }
}