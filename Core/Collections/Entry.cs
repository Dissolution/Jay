using System;
using System.Collections.Generic;
using Jay.Comparison;

namespace Jay.Collections
{
    /// <summary>
    /// A <see cref="Key"/> / <see cref="Value"/> entry in a dictionary
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type"/> of the key</typeparam>
    /// <typeparam name="TValue">The <see cref="Type"/> of the value</typeparam>
    public readonly struct Entry<TKey, TValue> : IEquatable<Entry<TKey, TValue>>
    {
        public static implicit operator Entry<TKey, TValue>(KeyValuePair<TKey, TValue> pair) =>
            new Entry<TKey, TValue>(pair.Key, pair.Value);

        public static implicit operator Entry<TKey, TValue>((TKey, TValue) tuple) =>
            new Entry<TKey, TValue>(tuple.Item1, tuple.Item2);

        public static bool operator ==(Entry<TKey, TValue> left, Entry<TKey, TValue> right) => left.Equals(right);
        public static bool operator !=(Entry<TKey, TValue> left, Entry<TKey, TValue> right) => !left.Equals(right);

        public readonly TKey Key;
        public readonly TValue Value;

        public Entry(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = this.Key;
            value = this.Value;
        }

        public bool Equals(TKey key) => EqualityComparer<TKey>.Default.Equals(this.Key, key);
        
        public bool Equals(TValue value) => EqualityComparer<TValue>.Default.Equals(this.Value, value);
        
        /// <inheritdoc />
        public bool Equals(Entry<TKey, TValue> entry)
        {
            return EqualityComparer<TKey>.Default.Equals(this.Key, entry.Key) && 
                   EqualityComparer<TValue>.Default.Equals(this.Value, entry.Value);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Entry<TKey, TValue> entry)
                return Equals(entry);
            if (obj is TKey key)
                return Equals(key);
            if (obj is TValue value)
                return Equals(value);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Hasher.Create<TKey, TValue>(Key, Value);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{Key},{Value}}}";
        }
    }
}