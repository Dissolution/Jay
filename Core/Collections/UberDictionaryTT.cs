using System.Diagnostics;
using System.Runtime.CompilerServices;
using Jay.Reflection;

namespace Jay.Collections
{
    public sealed class TestB914E146
    {
        public TestB914E146()
        {
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            dict["abc"] = "Alphabet";


        }
    }


    public struct Entry<TKey, TValue>
    {
        /// <summary>
        /// 0-based index of next entry in chain: -1 means end of chain
        /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        internal int Next;

        internal readonly uint HashCode;

        public readonly TKey Key;
        public TValue Value;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)HashCode;
        }

        public override string ToString()
        {
            return $"<{Key},{Value}>";
        }
    }

  

    public class UberDictionary<TKey, TValue> : IEnumerable<Entry<TKey, TValue>>
    {
        private const int StartOfFreeList = -3;

        public sealed class KeySet //: IReadOnlySet<TKey>, IReadOnlyCollection<TKey>, IEnumerable<TKey>
        {
            private readonly UberDictionary<TKey, TValue> _dictionary;

            public KeySet(UberDictionary<TKey, TValue> dictionary)
            {
                ArgumentNullException.ThrowIfNull(dictionary);
                _dictionary = dictionary;
            }
        }

        public sealed class ValueCollection //: IReadOnlyCollection<TValue>, IEnumerable<TValue>
        {
            private readonly UberDictionary<TKey, TValue> _dictionary;

            public ValueCollection(UberDictionary<TKey, TValue> dictionary)
            {
                ArgumentNullException.ThrowIfNull(dictionary);
                _dictionary = dictionary;
            }
        }

        public struct UberEnumerator : IEnumerator<Entry<TKey, TValue>>, IEnumerator
        {
            private readonly UberDictionary<TKey, TValue> _dictionary;

            public UberEnumerator(UberDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                Current = default;
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public Entry<TKey, TValue> Current { get; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }




        private int[]? _buckets;
        private Entry<TKey, TValue>[]? _entries;

        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
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
            var entries = new Entry<TKey, TValue>[size];

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
            ref Entry<TKey, TValue> entry = ref Danger.NullRef<Entry<TKey, TValue>>();
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

        public bool TryAdd(Entry<TKey, TValue> entry)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Entry<TKey, TValue>> entries)
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

        public bool Contains(Entry<TKey, TValue> entry, IEqualityComparer<TValue>? valueComparer = null)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(TKey key) => throw new NotImplementedException();

        public bool TryRemove(TKey key, TValue value, IEqualityComparer<TValue>? valueComparer = null) =>
            throw new NotImplementedException();

        public bool TryRemove(Entry<TKey, TValue> entry, IEqualityComparer<TValue>? valueComparer = null)
            => throw new NotImplementedException();

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public UberEnumerator GetEnumerator => new UberEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new UberEnumerator(this);

        IEnumerator<Entry<TKey, TValue>> IEnumerable<Entry<TKey, TValue>>.GetEnumerator() => new UberEnumerator(this);
    }
}
