using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jay.Collections
{
    public class TinyListDictionary<TKey, TValue> : ICollection<Entry<TKey, TValue>>
    {
        private readonly List<Entry<TKey, TValue>> _entries;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly IEqualityComparer<TValue> _valueComparer;

        public TValue this[TKey key]
        {
            get
            {
                for (var i = 0; i < _entries.Count; i++)
                {
                    var entry = _entries[i];
                    if (_keyComparer.Equals(key, entry.Key))
                        return entry.Value;
                }
                throw new ArgumentException($"Key '{key}' is not present", nameof(key));
            }
        }

        public Entry<TKey, TValue> this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count)
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {Count - 1}");
                return _entries[index];
            }
        }
        public int Count => _entries.Count;

        /// <inheritdoc />
        bool ICollection<Entry<TKey, TValue>>.IsReadOnly => false;

        public TinyListDictionary(int capacity = 0,
                                  IEqualityComparer<TKey>? keyComparer = null,
                                  IEqualityComparer<TValue>? valueComparer = null)
        {
            if (capacity <= 0)
            {
                _entries = new List<Entry<TKey, TValue>>(0);
            }
            else
            {
                _entries = new List<Entry<TKey, TValue>>(capacity);
            }

            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        private bool Equals(Entry<TKey, TValue> left, Entry<TKey, TValue> right)
        {
            return _keyComparer.Equals(left.Key, right.Key) &&
                   _valueComparer.Equals(left.Value, right.Value);
        }
        
        private bool Equals(Entry<TKey, TValue> left, TKey rightKey, TValue rightValue)
        {
            return _keyComparer.Equals(left.Key, rightKey) &&
                   _valueComparer.Equals(left.Value, rightValue);
        }
        
        public bool TryAdd(Entry<TKey, TValue> entry)
        {
            if (ContainsKey(entry.Key))
                return false;
            _entries.Add(entry);
            return true;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (ContainsKey(key))
                return false;
            _entries.Add(new Entry<TKey, TValue>(key, value));
            return true;
        }

        
        /// <inheritdoc />
        void ICollection<Entry<TKey, TValue>>.Add(Entry<TKey, TValue> entry)
        {
            AddOrUpdate(entry);
        }

        public void AddOrUpdate(Entry<TKey, TValue> entry)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, entry.Key))
                {
                    _entries[i] = entry;
                    return;
                }
            }
            _entries.Add(entry);
        }
        
        public void AddOrUpdate(TKey key, TValue value)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, value);
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, value));
        }

        public void AddOrUpdate(TKey key, TValue addValue, Func<TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValue));
        }
        
        public void AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(key, e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValue));
        }
        
        public void AddOrUpdate(TKey key, Func<TValue> addValueFactory, Func<TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValueFactory()));
        }
        
        public void AddOrUpdate(TKey key, Func<TValue> addValueFactory, Func<TKey, TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(key, e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValueFactory()));
        }

        public void AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValueFactory(key)));
        }
        
        public void AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValue)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (_keyComparer.Equals(e.Key, key))
                {
                    _entries[i] = new Entry<TKey, TValue>(key, updateValue(key, e.Value));
                    return;
                }
            }
            _entries.Add(new Entry<TKey, TValue>(key, addValueFactory(key)));
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(_entries[i].Key, key))
                {
                    value = _entries[i].Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
        
        public TValue GetOrDefault(TKey key, TValue @default = default(TValue))
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(_entries[i].Key, key))
                    return _entries[i].Value;
            }
            return @default;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(_entries[i].Key, key))
                    return _entries[i].Value;
            }
            _entries.Add(new Entry<TKey, TValue>(key, value));
            return value;
        }
        
        public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(_entries[i].Key, key))
                    return _entries[i].Value;
            }
            var value = valueFactory();
            _entries.Add(new Entry<TKey, TValue>(key, value));
            return value;
        }
        
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(_entries[i].Key, key))
                    return _entries[i].Value;
            }
            var value = valueFactory(key);
            _entries.Add(new Entry<TKey, TValue>(key, value));
            return value;
        }


        /// <inheritdoc />
        public void Clear()
        {
            _entries.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _entries.Any(entry => _keyComparer.Equals(entry.Key, key));
        }

        public bool ContainsValue(TValue value)
        {
            return _entries.Any(entry => _valueComparer.Equals(entry.Value, value));
        }
        
        /// <inheritdoc />
        public bool Contains(Entry<TKey, TValue> entry)
        {
            return _entries.Any(e => e.Equals(entry));
        }
        
        public bool Contains(TKey key, TValue value)
        {
            return _entries.Any(entry => Equals(entry, key, value));
        }

        /// <inheritdoc />
        void ICollection<Entry<TKey, TValue>>.CopyTo(Entry<TKey, TValue>[] array, int arrayIndex) 
            => _entries.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        bool ICollection<Entry<TKey, TValue>>.Remove(Entry<TKey, TValue> entry) => TryRemove(entry);

        public bool TryRemove(TKey key)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(key, _entries[i].Key))
                {
                    _entries.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_keyComparer.Equals(key, _entries[i].Key))
                {
                    value = _entries[i].Value;
                    _entries.RemoveAt(i);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryRemove(Entry<TKey, TValue> entry)
        {
            for (var i = 0; i < Count; i++)
            {
                if (Equals(_entries[i], entry))
                {
                    _entries.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool TryRemove(TKey key, TValue value)
        {
            for (var i = 0; i < Count; i++)
            {
                if (Equals(_entries[i], key, value))
                {
                    _entries.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        
        /// <inheritdoc />
        public IEnumerator<Entry<TKey, TValue>> GetEnumerator() => _entries.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}