using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Reflection;

namespace Jay.Collections;

public readonly struct UberEntry<TKey, TValue> : IEquatable<UberEntry<TKey, TValue>>
{
    public readonly TKey Key;
    public readonly TValue Value;

    internal UberEntry(TKey key, TValue value)
    {
        this.Key = key;
        this.Value = value;
    }
    public void Deconstruct(out TKey key, out TValue value)
    {
        key = this.Key;
        value = this.Value;
    }

    public bool Equals(UberEntry<TKey, TValue> entry)
    {
        return EqualityComparer<TKey>.Default.Equals(this.Key, entry.Key) &&
               EqualityComparer<TValue>.Default.Equals(this.Value, entry.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is UberEntry<TKey, TValue> entry && Equals(entry);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine<TKey, TValue>(Key, Value);
    }

    public override string ToString()
    {
        return $"{Key}: {Value}";
    }
}


  

public class UberDictionary<TKey, TValue> : IEnumerable<UberEntry<TKey, TValue>>
{
    public sealed class KeySet : IReadOnlySet<TKey>, IReadOnlyCollection<TKey>, IEnumerable<TKey>
    {
        private readonly Dictionary<TKey, TValue>.KeyCollection _implKeyCollection;

        public int Count => _implKeyCollection.Count;

        public KeySet(UberDictionary<TKey, TValue> dictionary)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            _implKeyCollection = dictionary._impl.Keys;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _implKeyCollection.GetEnumerator();
        }
        public IEnumerator<TKey> GetEnumerator()
        {
            return _implKeyCollection.GetEnumerator();
        }
        public bool Contains(TKey key)
        {
            return (_implKeyCollection as ICollection<TKey>).Contains(key);
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
    }

    public sealed class ValueCollection : IReadOnlyCollection<TValue>, IEnumerable<TValue>
    {
        private readonly UberDictionary<TKey, TValue> _dictionary;

        public int Count => _dictionary.Count;

        public ValueCollection(UberDictionary<TKey, TValue> dictionary)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            _dictionary = dictionary;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
        public IEnumerator<TValue> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
    }

    public struct UberEnumerator : IEnumerator<UberEntry<TKey, TValue>>, IEnumerator
    {
        private readonly UberDictionary<TKey, TValue> _dictionary;
        private Dictionary<TKey, TValue>.Enumerator _enumerator;

        object IEnumerator.Current => this.Current;

        public UberEntry<TKey, TValue> Current
        {
            get
            {
                (TKey key, TValue value) = _enumerator.Current;
                return new(key, value);
            }
        }
        
        public UberEnumerator(UberDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _enumerator = _dictionary._impl.GetEnumerator();
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            (_enumerator as IEnumerator).Reset();
        }

        void IDisposable.Dispose()
        {
            _enumerator.Dispose();
        }
    }


    private readonly Dictionary<TKey, TValue> _impl;

    private KeySet? _keys;
    private ValueCollection? _values;
    
    protected readonly IEqualityComparer<TKey> _keyComparer;


    public ref TValue this[TKey key]
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count - _freeCount;
    }

    public KeySet Keys => (_keys ??= new KeySet(this));

    public ValueCollection Values => (_values ??= new ValueCollection(this));




    public UberDictionary(int capacity = 0, IEqualityComparer<TKey>? keyComparer = null)
    {
        if (capacity > 0)
        {
            Initialize(capacity);
        }

        _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int GetBucket(uint hashCode)
    {
        int[] buckets = _buckets!;
        return ref buckets[hashCode % (uint)buckets.Length];
    }

    private int Initialize(int capacity)
    {
        int size = Hasher.GetNextLargerPrime(capacity);
        int[] buckets = new int[size];
        var entries = new DictEntry<TKey, TValue>[size];

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _freeList = -1;
#if TARGET_64BIT
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
#endif
        _buckets = buckets;
        _entries = entries;

        return size;
    }

    internal ref TValue FindValue(TKey key)
    {
        ref DictEntry<TKey, TValue> entry = ref Danger.NullRef<DictEntry<TKey, TValue>>();
        if (_buckets != null)
        {
            Debug.Assert(_entries != null, "expected entries to be != null");
            var comparer = _keyComparer;
            uint hashCode;
            if (key is null)
            {
                hashCode = 0U;
            }
            else
            {
                hashCode = (uint)comparer.GetHashCode(key);
            }
                
            int i = GetBucket(hashCode);
            var entries = _entries;
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
                if (entry.HashCode == hashCode && comparer.Equals(entry.Key, key))
                {
                    goto ReturnFound;
                }

                i = entry.Next;

                collisionCount++;
            } while (collisionCount <= (uint)entries.Length);

            // The chain of entries forms a loop; which means a concurrent update has happened.
            // Break out of the loop and throw, rather than looping forever.
            goto ConcurrentOperation;
        }

        goto ReturnNotFound;

        ConcurrentOperation:
        throw new Exception("ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported())");
        ReturnFound:
        ref TValue value = ref entry.Value;
        Return:
        return ref value;
        ReturnNotFound:
        value = ref Danger.NullRef<TValue>();
        goto Return;
    }


    public bool TryAdd(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }

    public bool TryAdd(DictEntry<TKey, TValue> entry)
    {
        throw new NotImplementedException();
    }

    public void AddRange(IEnumerable<DictEntry<TKey, TValue>> entries)
    {

    }

    public bool ContainsKey(TKey key)
    {
        throw new NotImplementedException();
    }

    public bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueComparer = null)
    {
        throw new NotImplementedException();
    }

    public bool Contains(TKey key, TValue value, IEqualityComparer<TValue>? valueComparer = null)
    {
        throw new NotImplementedException();
    }

    public bool Contains(DictEntry<TKey, TValue> entry, IEqualityComparer<TValue>? valueComparer = null)
    {
        throw new NotImplementedException();
    }

    public bool TryRemove(TKey key) => throw new NotImplementedException();

    public bool TryRemove(TKey key, TValue value, IEqualityComparer<TValue>? valueComparer = null) =>
        throw new NotImplementedException();

    public bool TryRemove(DictEntry<TKey, TValue> entry, IEqualityComparer<TValue>? valueComparer = null)
        => throw new NotImplementedException();

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public UberEnumerator GetEnumerator => new UberEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new UberEnumerator(this);

    IEnumerator<DictEntry<TKey, TValue>> IEnumerable<DictEntry<TKey, TValue>>.GetEnumerator() => new UberEnumerator(this);
}