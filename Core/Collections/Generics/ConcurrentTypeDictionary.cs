using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public sealed class ConcurrentTypeDictionary<TValue> : ITypeDictionary<TValue>,
                                                           IReadOnlyTypeDictionary<TValue>,
                                                           IReadOnlyCollection<KeyValuePair<Type, TValue>>,
                                                           IEnumerable<KeyValuePair<Type, TValue>>,
                                                           IEnumerable
    {
        private readonly ConcurrentDictionary<Type, TValue> _dictionary;

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        private ReadOnlyCollectionAdapter<Type>? _types;
        private ReadOnlyCollectionAdapter<TValue>? _values;

        /// <inheritdoc />
        public IReadOnlyCollection<Type> Types => (_types ??= new ReadOnlyCollectionAdapter<Type>(_dictionary.Keys));

        /// <inheritdoc />
        public IReadOnlyCollection<TValue> Values => (_values ??= new ReadOnlyCollectionAdapter<TValue>(_dictionary.Values));
        
        public ConcurrentTypeDictionary()
        {
            _dictionary = new ConcurrentDictionary<Type, TValue>();
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
        public bool TryRemove<T>() => _dictionary.TryRemove(typeof(T), out _);

        /// <inheritdoc />
        public bool TryRemove(Type type) => _dictionary.TryRemove(type, out _);

        /// <inheritdoc />
        public bool TryRemove<T>([MaybeNullWhen(false)] out TValue removedValue)
            => _dictionary.TryRemove(typeof(T), out removedValue);

        public void Set(Type type, TValue value)
        {
            _dictionary[type] = value;
        }
        
        /// <inheritdoc />
        public void Set<T>(TValue value)
        {
            _dictionary[typeof(T)] = value;
        }

        /// <inheritdoc />
        public TValue GetOrAdd<T>(TValue value) => _dictionary.GetOrAdd(typeof(T), value);

        /// <inheritdoc />
        public TValue GetOrAdd<T>(Func<TValue> valueFactory) => _dictionary.GetOrAdd(typeof(T), _ => valueFactory());

        /// <inheritdoc />
        public TValue GetOrAdd(Type type, Func<Type, TValue> valueFactory) => _dictionary.GetOrAdd(type, valueFactory);

        /// <inheritdoc />
        public TValue AddOrUpdate<T>(TValue newValue, Func<TValue, TValue> updateValue)
        {
            return _dictionary.AddOrUpdate(typeof(T), newValue, (type, existing) => updateValue(existing));
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<T>(Func<TValue> newValueFactory, Func<TValue, TValue> updateValue)
        {
            return _dictionary.AddOrUpdate(typeof(T), type => newValueFactory(), (type, existing) => updateValue(existing));
        }

        /// <inheritdoc />
        public TValue AddOrUpdate(Type type, Func<Type, TValue> newValueFactory, Func<Type, TValue, TValue> updateValue)
        {
            return _dictionary.AddOrUpdate(type, newValueFactory, updateValue);
        }

        /// <inheritdoc />
        public IReadOnlyTypeDictionary<TValue> AsReadOnly()
        {
            return this;
        }
     
        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}