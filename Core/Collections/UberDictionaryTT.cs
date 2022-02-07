using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public readonly uint HashCode;
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

  

    public class UberDictionary<TKey, TValue>
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

        public KeySet Keys => (_keys ??= new KeySet(this));

        public ValueCollection Values => (_values ??= new ValueCollection(this));


        public UberDictionary(int capacity = 0, IEqualityComparer<TKey>? keyComparer = null)
        {
            if (capacity > 0)
            {
                //Initialize(capacity);
            }

            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(Entry<TKey, TValue> entry)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }



        public void AddRange(IEnumerable<Entry<TKey, TValue>> entries)
        {

        }
    }
}
