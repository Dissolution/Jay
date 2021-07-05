using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public sealed class TypeDictionary<TValue> : ITypeDictionary<TValue>,
                                                 IReadOnlyTypeDictionary<TValue>,
                                                 IReadOnlyCollection<KeyValuePair<Type, TValue>>,
                                                 IEnumerable<KeyValuePair<Type, TValue>>,
                                                 IEnumerable
    {
        private readonly Dictionary<Type, TValue> _dictionary;

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Types => _dictionary.Keys;

        /// <inheritdoc />
        public IReadOnlyCollection<TValue> Values => _dictionary.Values;
        
        public TypeDictionary(int capacity = 64)
        {
            _dictionary = new Dictionary<Type, TValue>(capacity);
        }

        /// <inheritdoc />
        public bool Contains<T>() => _dictionary.ContainsKey(typeof(T));

        /// <inheritdoc />
        public bool Contains(Type type) => _dictionary.ContainsKey(type);

        /// <inheritdoc />
        public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value)
            => _dictionary.TryGetValue(typeof(T), out value);

        /// <inheritdoc />
        public bool TryAdd<T>(TValue value) => _dictionary.TryAdd(typeof(T), value);

        /// <inheritdoc />
        public bool TryRemove<T>() => _dictionary.Remove(typeof(T));

        /// <inheritdoc />
        public bool TryRemove(Type type) => _dictionary.Remove(type);

        /// <inheritdoc />
        public bool TryRemove<T>([MaybeNullWhen(false)] out TValue removedValue) 
            => _dictionary.Remove(typeof(T), out removedValue);

        /// <inheritdoc />
        public bool TryUpdate<T>(TValue comparisonValue, TValue newValue)
        {
            if (_dictionary.TryGetValue(typeof(T), out var existingValue))
            {
                if (EqualityComparer<TValue>.Default.Equals(existingValue, comparisonValue))
                {
                    _dictionary[typeof(T)] = newValue;
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public bool TryUpdate<T>(Func<TValue, TValue> updateValue)
        {
            if (_dictionary.TryGetValue(typeof(T), out var existingValue))
            {
                _dictionary[typeof(T)] = updateValue(existingValue);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public bool TryUpdate(Type type, Func<Type, TValue, TValue> updateValue)
        {
            if (_dictionary.TryGetValue(type, out var existingValue))
            {
                _dictionary[type] = updateValue(type, existingValue);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void Set<T>(TValue value)
        {
            _dictionary[typeof(T)] = value;
        }

        /// <inheritdoc />
        public TValue GetOrAdd<T>(TValue value)
        {
            if (_dictionary.TryGetValue(typeof(T), out var existingValue))
            {
                return existingValue;
            }
            _dictionary[typeof(T)] = value;
            return value;
        }

        /// <inheritdoc />
        public TValue GetOrAdd<T>(Func<TValue> valueFactory)
        {
            if (_dictionary.TryGetValue(typeof(T), out var value))
            {
                return value;
            }
            value = valueFactory();
            _dictionary[typeof(T)] = value;
            return value;
        }

        /// <inheritdoc />
        public TValue GetOrAdd(Type type, Func<Type, TValue> valueFactory)
        {
            if (_dictionary.TryGetValue(type, out var value))
            {
                return value;
            }
            value = valueFactory(type);
            _dictionary[type] = value;
            return value;
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<T>(TValue newValue, Func<TValue, TValue> updateValue)
        {
            TValue? value;
            if (_dictionary.TryGetValue(typeof(T), out value))
            {
                value = updateValue(value);
            }
            else
            {
                value = newValue;
            }
            _dictionary[typeof(T)] = value;
            return value;
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<T>(Func<TValue> newValueFactory, Func<TValue, TValue> updateValue)
        {
            TValue? value;
            if (_dictionary.TryGetValue(typeof(T), out value))
            {
                value = updateValue(value);
            }
            else
            {
                value = newValueFactory();
            }
            _dictionary[typeof(T)] = value;
            return value;
        }

        /// <inheritdoc />
        public TValue AddOrUpdate(Type type, Func<Type, TValue> newValueFactory, Func<Type, TValue, TValue> updateValue)
        {
            TValue? value;
            if (_dictionary.TryGetValue(type, out value))
            {
                value = updateValue(type, value);
            }
            else
            {
                value = newValueFactory(type);
            }
            _dictionary[type] = value;
            return value;
        }

        /// <inheritdoc />
        public IReadOnlyTypeDictionary<TValue> AsReadOnly()
        {
            _dictionary.TrimExcess();
            return this;
        }
     
        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}