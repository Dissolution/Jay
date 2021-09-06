using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jay.Collections
{
    public interface IReadOnlyListDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>,
                                                             IReadOnlyList<KeyValuePair<TKey, TValue>>,
                                                             IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
                                                             IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        
    }

    public interface IListDictionary<TKey, TValue> : IReadOnlyListDictionary<TKey, TValue>,
                                                     IDictionary<TKey, TValue>,
                                                     IList<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        
    }

    public class ListDictionary<TKey, TValue> : IListDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly List<TKey> _keys;
      
        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new ArgumentOutOfRangeException(nameof(key));
            }
            set
            {
                if (_dictionary.TryAdd(key, value))
                {
                    _keys.Add(key);
                }
                else
                {
                    _dictionary[key] = value;
                }
            }
        }

        /// <inheritdoc />
        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                if ((uint) index < (uint) _keys.Count)
                {
                    var key = _keys[index];
                    return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            set
            {
                if ((uint) index < (uint) _keys.Count)
                {
                    _dictionary[_keys[index]] = value.Value;
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <inheritdoc />
        public int Count => _keys.Count;

        public IReadOnlyList<TKey> Keys => _keys;
        public IReadOnlyList<TValue> Values => _dictionary.Values.ToList();

        public IEqualityComparer<TKey> KeyComparer { get; }
        public IEqualityComparer<TValue> ValueComparer { get; }

        /// <inheritdoc />
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;
        /// <inheritdoc />
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys;

        /// <inheritdoc />
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;
        /// <inheritdoc />
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

        public ListDictionary(IEqualityComparer<TKey>? keyComparer = null,
                              IEqualityComparer<TValue>? valueComparer = null)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer: keyComparer);
            _keys = new List<TKey>();
            this.KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            this.ValueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        public Result<int> TryAdd(KeyValuePair<TKey, TValue> pair)
        {
            if (_dictionary.TryAdd(pair.Key, pair.Value))
            {
                int index = _keys.Count;
                _keys.Add(pair.Key);
                return index;
            }
            return new ArgumentException($"The given {pair}'s key '{pair.Key}' is already present",
                                         nameof(pair));
        }
        
        public Result<int> TryAdd(TKey key, TValue value)
        {
            if (_dictionary.TryAdd(key, value))
            {
                int index = _keys.Count;
                _keys.Add(key);
                return index;
            }
            return new ArgumentException($"The given key '{key}' is already present",
                                         nameof(key));
        }
        
        /// <inheritdoc />
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => TryAdd(key, value).ThrowIfFailed();

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair) => TryAdd(pair).ThrowIfFailed();
        
        /// <inheritdoc />
        void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> pair)
        {
            if (_dictionary.TryAdd(pair.Key, pair.Value))
            {
                _keys.Insert(index, pair.Key);
            }
            throw new ArgumentException($"The given {pair}'s key '{pair.Key}' is already present",
                                         nameof(pair));
        }

        public Result<int> TryRemove(TKey key)
        {
            if (_dictionary.Remove(key))
            {
                var index = _keys.IndexOf(key);
                Debug.Assert(index >= 0);
                _keys.RemoveAt(index);
                return index;
            }
            return new Result<int>(false, -1, new ArgumentException($"The given key '{key}' is not present"));
        }

        public Result<int> TryRemove(KeyValuePair<TKey, TValue> pair)
        {
            if (_dictionary.TryGetValue(pair.Key, out var value))
            {
                if (ValueComparer.Equals(value, pair.Value))
                {
                    var removed = _dictionary.Remove(pair.Key);
                    Debug.Assert(removed);
                    var index = _keys.IndexOf(pair.Key);
                    Debug.Assert(index >= 0);
                    _keys.RemoveAt(index);
                    return index;
                }
                return new Result<int>(false, -1, new ArgumentException($"The given key '{pair.Key}' is present, but its value is not the given value"));
            }
            return new Result<int>(false, -1, new ArgumentException($"The given key '{pair.Key}' is not present"));
        }

        public Result<int> TryRemove(TKey key, TValue value)
        {
            if (_dictionary.TryGetValue(key, out var existingValue))
            {
                if (ValueComparer.Equals(existingValue, value))
                {
                    var removed = _dictionary.Remove(key);
                    Debug.Assert(removed);
                    var index = _keys.IndexOf(key);
                    Debug.Assert(index >= 0);
                    _keys.RemoveAt(index);
                    return index;
                }
                return new Result<int>(false, -1, new ArgumentException($"The given key '{key}' is present, but its value is not the given value"));
            }
            return new Result<int>(false, -1, new ArgumentException($"The given key '{key}' is not present"));
        }
        
        
        /// <inheritdoc />
        bool IDictionary<TKey, TValue>.Remove(TKey key) => TryRemove(key);

        /// <inheritdoc />
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pair) => TryRemove(pair);

    

        public bool TryRemoveAt(int index, out KeyValuePair<TKey, TValue> removedPair)
        {
            if ((uint) index < (uint) _keys.Count)
            {
                var key = _keys[index];
                _keys.RemoveAt(index);
                var removed = _dictionary.Remove(key, out var value);
                Debug.Assert(removed);
                removedPair = new KeyValuePair<TKey, TValue>(key, value);
                return true;
            }

            removedPair = default;
            return false;
        }
        
        /// <inheritdoc />
        void IList<KeyValuePair<TKey, TValue>>.RemoveAt(int index)
        {
            if (!TryRemoveAt(index, out _))
                throw new IndexOutOfRangeException();
        }
        
        
        /// <inheritdoc />
        public void Clear()
        {
            _keys.Clear();
            _dictionary.Clear();
        }
        
        /// <inheritdoc />
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        
        /// <inheritdoc />
        public bool TryGetValue(int index, out KeyValuePair<TKey, TValue> pair)
        {
            if ((uint) index < (uint) _keys.Count)
            {
                var key = _keys[index];
                pair = new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
                return true;
            }

            pair = default;
            return false;
        }
        
        
        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dictionary.Values.Any(v => ValueComparer.Equals(v, value));
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return _dictionary.TryGetValue(pair.Key, out var existingValue) && ValueComparer.Equals(existingValue, pair.Value);
        }

        public bool Contains(TKey key, TValue value)
        {
            return _dictionary.TryGetValue(key, out var existingValue) && ValueComparer.Equals(existingValue, value);
        }

        public int IndexOf(TKey key)
        {
            for (var i = 0; i < _keys.Count; i++)
            {
                if (KeyComparer.Equals(key, _keys[i]))
                    return i;
            }
            return -1;
        }
        
        /// <inheritdoc />
        public int IndexOf(KeyValuePair<TKey, TValue> pair)
        {
            for (var i = 0; i < _keys.Count; i++)
            {
                if (KeyComparer.Equals(pair.Key, _keys[i]))
                {
                    if (ValueComparer.Equals(pair.Value, _dictionary[pair.Key]))
                    {
                        return i;
                    }
                    return -1;
                }
            }
            return -1;
        }


        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        
        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in _keys)
            {
                yield return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}