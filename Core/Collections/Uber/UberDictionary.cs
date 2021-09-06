using System;
using System.Collections.Generic;

namespace Jay.Collections.Uber
{
    public enum UberMode
    {
        Throw = 0,
        ReturnDefault = 1,
        UseValueFactory = 2,
    }

    public interface IReadOnlyUberDictionary<TKey, TValue> : IEnumerable<Entry<TKey, TValue>>
    {
        TValue this[TKey key] { get; }
        
        int Count { get; }

        IEqualityComparer<TKey> KeyComparer { get; }
        
        IReadOnlySet<TKey> Keys { get; }
        IReadOnlyCollection<TValue> Values { get; }
        
        bool ContainsKey(TKey key);
        bool ContainsValue(TValue value);
        bool ContainsValue(TValue value, IEqualityComparer<TValue> valueComparer);
        bool Contains(Entry<TKey, TValue> entry);
        bool Contains(TKey key, TValue value);
    }
    
    public interface IUberDictionary<TKey, TValue> : IReadOnlyUberDictionary<TKey, TValue>
    {
        new TValue this[TKey key] { get; set; }

        bool TryGetValue(TKey key, out TValue value);

        TValue GetOrAdd(TKey key, TValue addValue);
        TValue GetOrAdd(TKey key, Func<TValue> valueFactory);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);

        bool TryAdd(Entry<TKey, TValue> entry);
        bool TryAdd(TKey key, TValue value);

        void AddOrUpdate(Entry<TKey, TValue> entry);
        void AddOrUpdate(TKey key, TValue value);
        void AddOrUpdate(TKey key, TValue addValue, Func<TValue, TValue> updateValue);
        void AddOrUpdate(TKey key, Func<TValue> addValue, Func<TValue, TValue> updateValue);
        void AddOrUpdate(TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValue);

        bool TryRemove(TKey key);
        bool TryRemove(TKey key, out TValue value);
        bool TryRemove(Entry<TKey, TValue> entry);
        bool TryRemove(TKey key, TValue value);

        void RemoveWhere(Func<Entry<TKey, TValue>, bool> predicate);
        void RemoveWhere(Func<Entry<TKey, TValue>, bool> predicate, out IReadOnlyList<Entry<TKey, TValue>> removedEntries);
    }
}