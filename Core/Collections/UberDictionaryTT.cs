using System.Diagnostics;
using Jay.Collections.Pooling;
using Jay.Exceptions;
using Jay.Utilities;

namespace Jay.Collections;

public static class KVP
{
    public static KVP<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) => new(key, value);
}

public readonly struct KVP<TKey, TValue>
{
    public readonly TKey Key;
    public readonly TValue Value;

    public KVP(TKey key, TValue value)
    {
        this.Key = key;
        this.Value = value;
    }
    public void Deconstruct(out TKey key, out TValue value)
    {
        key = this.Key;
        value = this.Value;
    }

    public override bool Equals(object? obj) => false;

    public override int GetHashCode() => Hasher.Combine(this.Key, this.Value);

    public override string ToString() => $"[{Key}, {Value}]";
}

public enum InsertionBehavior
{
    None,
    OverwriteExisting,
    ThrowOnExisting,
}

partial class UberDictionary<TKey, TValue>
{
    [DebuggerDisplay("Count = {Count}")]
    public sealed class KeyCollection : IReadOnlySet<TKey>, IReadOnlyCollection<TKey>, IEnumerable<TKey>
    {
        private readonly UberDictionary<TKey, TValue> _dictionary;

        public int Count => _dictionary.Count;

        public KeyCollection(UberDictionary<TKey, TValue> dictionary)
        {
            Validate.NotNull(dictionary);
            _dictionary = dictionary;
        }

        public bool Contains(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool IsProperSubsetOf(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }
        public bool IsProperSupersetOf(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }
        public bool IsSubsetOf(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }
        public bool IsSupersetOf(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }
        public bool Overlaps(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }
        public bool SetEquals(IEnumerable<TKey> other)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TKey[] array, int index = 0)
        {
            Validate.NotNull(array);
            Validate.Index(array.Length, index);
            if (array.Length - index < _dictionary.Count)
            {
                throw new ArgumentException("Array is too small to contain the keys", nameof(array));
            }
            int count = _dictionary._count;
            Entry[]? entries = _dictionary._entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].Next >= -1)
                {
                    array[index++] = entries[i].Key;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => new KeyCollectionEnumerator(_dictionary);
        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new KeyCollectionEnumerator(_dictionary);
        public KeyCollectionEnumerator GetEnumerator() => new KeyCollectionEnumerator(_dictionary);

        public struct KeyCollectionEnumerator : IEnumerator<TKey>, IEnumerator
        {
            private readonly UberDictionary<TKey, TValue> _dictionary;
            private int _index;
            private readonly int _version;
            private TKey? _currentKey;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index != 0 && (_index != _dictionary._count + 1)) return _currentKey;
                    throw new EnumerationFailedException();
                }
            }

            public TKey Current => _currentKey!;

            internal KeyCollectionEnumerator(UberDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _currentKey = default;
            }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    throw new EnumerationFailedException();
                }

                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref Entry entry = ref _dictionary._entries![_index++];

                    if (entry.Next >= -1)
                    {
                        _currentKey = entry.Key;
                        return true;
                    }
                }

                _index = _dictionary._count + 1;
                _currentKey = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    throw new EnumerationFailedException();
                }

                _index = 0;
                _currentKey = default;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public sealed class ValueCollection : IReadOnlyCollection<TValue>, IEnumerable<TValue>
    {
        private readonly UberDictionary<TKey, TValue> _dictionary;

        public ValueCollection(UberDictionary<TKey, TValue> dictionary)
        {
            Validate.NotNull(dictionary);
            _dictionary = dictionary;
        }

        public void CopyTo(TValue[] array, int index)
        {
            Validate.NotNull(array);
            Validate.Index(array.Length, index);
            if (array.Length - index < _dictionary.Count)
            {
                throw new ArgumentException("Array is too small to contain the values", nameof(array));
            }

            int count = _dictionary._count;
            Entry[]? entries = _dictionary._entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].Next >= -1)
                    array[index++] = entries[i].Value;
            }
        }

        public int Count => _dictionary.Count;



        IEnumerator IEnumerable.GetEnumerator() => new ValueCollectionEnumerator(_dictionary);
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new ValueCollectionEnumerator(_dictionary);
        public ValueCollectionEnumerator GetEnumerator() => new ValueCollectionEnumerator(_dictionary);

        public struct ValueCollectionEnumerator : IEnumerator<TValue>, IEnumerator
        {
            private readonly UberDictionary<TKey, TValue> _dictionary;
            private int _index;
            private readonly int _version;
            private TValue? _currentValue;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index != 0 && (_index != _dictionary._count + 1)) return _currentValue;
                    throw new EnumerationFailedException();

                }
            }

            public TValue Current => _currentValue!;

            internal ValueCollectionEnumerator(UberDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _currentValue = default;
            }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    throw new EnumerationFailedException();
                }

                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref Entry entry = ref _dictionary._entries![_index++];

                    if (entry.Next >= -1)
                    {
                        _currentValue = entry.Value;
                        return true;
                    }
                }
                _index = _dictionary._count + 1;
                _currentValue = default;
                return false;
            }


            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    throw new EnumerationFailedException();
                }

                _index = 0;
                _currentValue = default;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
    
    private struct Entry
    {
        public uint HashCode;
        /// <summary>
        /// 0-based index of next entry in chain: -1 means end of chain
        /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        public int Next;
        public TKey Key; // Key of entry
        public TValue Value; // Value of entry
    }
    
    public struct UberDictionaryEnumerator : IEnumerator<KVP<TKey, TValue>>, IEnumerator
    {
        private readonly UberDictionary<TKey, TValue> _dictionary;
        private readonly int _version;
        private int _index;
        private KVP<TKey, TValue> _current;

        object? IEnumerator.Current => _current;
        public KVP<TKey, TValue> Current => _current;
        
        internal UberDictionaryEnumerator(UberDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _version = dictionary._version;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            if (_version != _dictionary._version)
            {
                throw new EnumerationFailedException();
            }

            // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
            // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
            while ((uint)_index < (uint)_dictionary._count)
            {
                ref Entry entry = ref _dictionary._entries![_index++];

                if (entry.Next >= -1)
                {
                    _current = new(entry.Key, entry.Value);
                    return true;
                }
            }

            _index = _dictionary._count + 1;
            _current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            if (_version != _dictionary._version)
            {
                //ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                throw new EnumerationFailedException();
            }

            _index = 0;
            _current = default;
        }
        
        void IDisposable.Dispose()
        {
        }
    }
}

[DebuggerDisplay("Count = {Count}")]
public sealed partial class UberDictionary<TKey, TValue> : IEnumerable<KVP<TKey, TValue>>
    /* Roughly:
    IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>
    */
{
    private const int StartOfFreeList = -3;
    

    private int[] _buckets;
    private Entry[] _entries;

    private int _count;
    private int _freeList;
    private int _freeCount;
    private int _version;


    private readonly IEqualityComparer<TKey> _keyComparer;
    private KeyCollection? _keys;
    private ValueCollection? _values;


    public TValue this[TKey key]
    {
        get
        {
            ref TValue value = ref FindValue(key);
            if (!Danger.IsNullRef(ref value))
            {
                return value;
            }
            throw new KeyNotFoundException();
        }
        set
        {
            bool modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
            Debug.Assert(modified);
        }
    }

    public int Count => _count - _freeCount;

    public IEqualityComparer<TKey> KeyComparer => _keyComparer;

    public KeyCollection Keys => _keys ??= new KeyCollection(this);

    public ValueCollection Values => _values ??= new ValueCollection(this);


    public UberDictionary(int capacity = 0, IEqualityComparer<TKey>? comparer = null)
    {
        if (capacity < 0)
            capacity = 0;
        if (capacity > Pool.ArrayMaxCapacity)
            capacity = Pool.ArrayMaxCapacity;
        int[] buckets = new int[capacity];
        Entry[] entries = new Entry[capacity];

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _freeList = -1;
        _buckets = buckets;
        _entries = entries;
        _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int GetBucket(uint hashCode)
    {
        int[] buckets = _buckets!;
        return ref buckets[hashCode % (uint)buckets.Length];
    }

    private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
    {
        Entry[] entries = _entries;
        IEqualityComparer<TKey> comparer = _keyComparer;
        uint hashCode = key is null ? 0U : (uint)comparer.GetHashCode(key);

        uint collisionCount = 0;
        ref int bucket = ref GetBucket(hashCode);
        int i = bucket - 1; // Value in _buckets is 1-based

        while (true)
        {
            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
            // Test uint in if rather than loop condition to drop range check for following array access
            if ((uint)i >= (uint)entries.Length)
            {
                break;
            }

            if (entries[i].HashCode == hashCode && comparer.Equals(entries[i].Key, key))
            {
                if (behavior == InsertionBehavior.OverwriteExisting)
                {
                    entries[i].Value = value;
                    return true;
                }

                if (behavior == InsertionBehavior.ThrowOnExisting)
                {
                    throw new InvalidOperationException("Duplicate Key");
                    //ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                }

                return false;
            }

            i = entries[i].Next;

            collisionCount++;
            if (collisionCount > (uint)entries.Length)
            {
                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                //                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                throw new EnumerationFailedException("Concurrent operations not supported");
            }
        }

        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            Debug.Assert((StartOfFreeList - entries[_freeList].Next) >= -1, "shouldn't overflow because `next` cannot underflow");
            _freeList = StartOfFreeList - entries[_freeList].Next;
            _freeCount--;
        }
        else
        {
            int count = _count;
            if (count == entries.Length)
            {
                Resize();
                bucket = ref GetBucket(hashCode);
            }
            index = count;
            _count = count + 1;
            entries = _entries;
        }

        ref Entry entry = ref entries![index];
        entry.HashCode = hashCode;
        entry.Next = bucket - 1; // Value in _buckets is 1-based
        entry.Key = key;
        entry.Value = value;
        bucket = index + 1; // Value in _buckets is 1-based
        _version++;

        return true;
    }

    internal ref TValue FindValue(TKey? key)
    {
        ref Entry entry = ref Danger.NullRef<Entry>();
        Debug.Assert(_entries != null, "expected entries to be != null");
        var keyComparer = _keyComparer;
        uint hashCode = key is null ? 0U : (uint)keyComparer.GetHashCode(key);
        int i = GetBucket(hashCode);
        Entry[] entries = _entries;
        uint collisionCount = 0;
        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
        do
        {
            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
            // Test in if to drop range check for following array access
            if ((uint)i >= (uint)entries.Length)
            {
                goto ReturnNotFound;
            }

            entry = ref entries[i];
            if (entry.HashCode == hashCode && keyComparer.Equals(entry.Key, key))
            {
                goto ReturnFound;
            }

            i = entry.Next;

            collisionCount++;
        } while (collisionCount <= (uint)entries.Length);

        // The chain of entries forms a loop; which means a concurrent update has happened.
        throw new EnumerationFailedException("Does not support concurrent operations");
    ReturnFound:
        ref TValue value = ref entry.Value;
    Return:
        return ref value;
    ReturnNotFound:
        value = ref Danger.NullRef<TValue>();
        goto Return;
    }

    private void CopyEntries(Entry[] entries, int count)
    {
        Entry[] newEntries = _entries;
        int newCount = 0;
        for (int i = 0; i < count; i++)
        {
            uint hashCode = entries[i].HashCode;
            if (entries[i].Next >= -1)
            {
                ref Entry entry = ref newEntries[newCount];
                entry = entries[i];
                ref int bucket = ref GetBucket(hashCode);
                entry.Next = bucket - 1; // Value in _buckets is 1-based
                bucket = newCount + 1;
                newCount++;
            }
        }

        _count = newCount;
        _freeCount = 0;
    }
    private void Resize() => Resize(_count * 2, false);

    private void Resize(int newSize, bool forceNewHashCodes)
    {
        // Value types never rehash
        Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType);
        Debug.Assert(_entries != null, "_entries should be non-null");
        Debug.Assert(newSize >= _entries.Length);

        Entry[] entries = new Entry[newSize];

        int count = _count;
        Array.Copy(_entries, entries, count);

        // if (!typeof(TKey).IsValueType && forceNewHashCodes)
        // {
        //     Debug.Assert(_keyComparer is NonRandomizedStringEqualityComparer);
        //     _keyComparer = (IEqualityComparer<TKey>)((NonRandomizedStringEqualityComparer)_keyComparer).GetRandomizedEqualityComparer();
        //
        //     for (int i = 0; i < count; i++)
        //     {
        //         if (entries[i].next >= -1)
        //         {
        //             entries[i].hashCode = (uint)_keyComparer.GetHashCode(entries[i].key);
        //         }
        //     }
        //
        //     if (ReferenceEquals(_keyComparer, EqualityComparer<TKey>.Default))
        //     {
        //         _keyComparer = null;
        //     }
        // }

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _buckets = new int[newSize];
        for (int i = 0; i < count; i++)
        {
            if (entries[i].Next >= -1)
            {
                ref int bucket = ref GetBucket(entries[i].HashCode);
                entries[i].Next = bucket - 1; // Value in _buckets is 1-based
                bucket = i + 1;
            }
        }

        _entries = entries;
    }
    
    
    

    /// <summary>
    /// Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    ///
    /// To allocate minimum size storage array, execute the following statements:
    ///
    /// dictionary.Clear();
    /// dictionary.TrimExcess();
    /// </remarks>
    public void TrimExcess() => TrimExcess(Count);

    /// <summary>
    /// Sets the capacity of this dictionary to hold up 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    /// </remarks>
    public void TrimExcess(int capacity)
    {
        if (capacity < Count)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        int newSize = capacity * 2;
        Entry[] oldEntries = _entries;
        int currentCapacity = oldEntries.Length;
        if (newSize >= currentCapacity)
        {
            return;
        }

        int oldCount = _count;
        _version++;
        CopyEntries(oldEntries, oldCount);
    }
    
    /// <summary>
    /// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    public int EnsureCapacity(int capacity)
    {
        int currentCapacity = _entries.Length;
        if (currentCapacity >= capacity)
        {
            return currentCapacity;
        }

        _version++;
        int newSize = (currentCapacity + capacity) * 2;
        Resize(newSize, forceNewHashCodes: false);
        return newSize;
    }
    

    public bool TryAdd(TKey key, TValue value) =>
        TryInsert(key, value, InsertionBehavior.None);


    public void Add(TKey key, TValue value)
    {
        bool modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
        Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
    }

    public bool TryAdd(KVP<TKey, TValue> kvp) => TryInsert(kvp.Key, kvp.Value, InsertionBehavior.None);

    public void Add(KVP<TKey, TValue> kvp)
    {
        bool modified = TryInsert(kvp.Key, kvp.Value, InsertionBehavior.ThrowOnExisting);
        Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.  
    }

    public bool ContainsKey(TKey key)
    {
        return !Danger.IsNullRef(ref FindValue(key));
    }

    public bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueComparer = default)
    {
        valueComparer ??= EqualityComparer<TValue>.Default;
        Entry[]? entries = _entries;
        for (int i = 0; i < _count; i++)
        {
            if (entries![i].Next >= -1 && valueComparer.Equals(entries[i].Value, value))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(KVP<TKey, TValue> kvp)
    {
        ref TValue value = ref FindValue(kvp.Key);
        if (!Danger.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, kvp.Value))
        {
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref TValue valRef = ref FindValue(key);
        if (!Danger.IsNullRef(ref valRef))
        {
            value = valRef;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryRemove(TKey key)
    {
        uint collisionCount = 0;
        var keyComparer = _keyComparer;
        uint hashCode = key is null ? 0U : (uint)keyComparer.GetHashCode(key);
        ref int bucket = ref GetBucket(hashCode);
        Entry[] entries = _entries;
        int last = -1;
        int i = bucket - 1; // Value in buckets is 1-based
        while (i >= 0)
        {
            ref Entry entry = ref entries[i];

            if (entry.HashCode == hashCode && keyComparer.Equals(entry.Key, key))
            {
                if (last < 0)
                {
                    bucket = entry.Next + 1; // Value in buckets is 1-based
                }
                else
                {
                    entries[last].Next = entry.Next;
                }

                Debug.Assert((StartOfFreeList - _freeList) < 0,
                    "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                entry.Next = StartOfFreeList - _freeList;

                if (TypeExtensions.IsReferenceOrContainsReferences<TKey>())
                {
                    entry.Key = default!;
                }

                if (TypeExtensions.IsReferenceOrContainsReferences<TValue>())
                {
                    entry.Value = default!;
                }

                _freeList = i;
                _freeCount++;
                return true;
            }

            last = i;
            i = entry.Next;

            collisionCount++;
            if (collisionCount > (uint)entries.Length)
            {
                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                //ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                throw new EnumerationFailedException("Concurrent operations not supported");
            }
        }
        return false;
    }

    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        uint collisionCount = 0;
        var keyComparer = _keyComparer;
        uint hashCode = key is null ? 0U : (uint)keyComparer.GetHashCode(key);
        ref int bucket = ref GetBucket(hashCode);
        Entry[] entries = _entries;
        int last = -1;
        int i = bucket - 1; // Value in buckets is 1-based
        while (i >= 0)
        {
            ref Entry entry = ref entries[i];

            if (entry.HashCode == hashCode && keyComparer.Equals(entry.Key, key))
            {
                if (last < 0)
                {
                    bucket = entry.Next + 1; // Value in buckets is 1-based
                }
                else
                {
                    entries[last].Next = entry.Next;
                }

                value = entry.Value;

                Debug.Assert((StartOfFreeList - _freeList) < 0,
                    "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                entry.Next = StartOfFreeList - _freeList;

                if (TypeExtensions.IsReferenceOrContainsReferences<TKey>())
                {
                    entry.Key = default!;
                }

                if (TypeExtensions.IsReferenceOrContainsReferences<TValue>())
                {
                    entry.Value = default!;
                }

                _freeList = i;
                _freeCount++;
                return true;
            }

            last = i;
            i = entry.Next;

            collisionCount++;
            if (collisionCount > (uint)entries.Length)
            {
                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                //ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                throw new EnumerationFailedException("Concurrent operations not supported");
            }
        }

        value = default;
        return false;
    }

    public bool TryRemove(KVP<TKey, TValue> kvp)
    {
        ref TValue value = ref FindValue(kvp.Key);
        if (!Danger.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, kvp.Value))
        {
            bool removed = TryRemove(kvp.Key);
            Debug.Assert(removed);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        int count = _count;
        if (count > 0)
        {
            Array.Clear(_buckets, 0, _buckets.Length);

            _count = 0;
            _freeList = -1;
            _freeCount = 0;
            Array.Clear(_entries, 0, count);
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => new UberDictionaryEnumerator(this);
    IEnumerator<KVP<TKey, TValue>> IEnumerable<KVP<TKey, TValue>>.GetEnumerator() => new UberDictionaryEnumerator(this);
    public UberDictionaryEnumerator GetEnumerator() => new UberDictionaryEnumerator(this);

  
}