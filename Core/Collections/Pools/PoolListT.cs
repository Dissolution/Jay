using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections.Pools
{
    public sealed class PoolList<T> : IList<T>, IReadOnlyList<T>,
                                      ICollection<T>, IReadOnlyCollection<T>,
                                      IEnumerable<T>, IDisposable
    {
        private T[] _items;
        private int _length;

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _length;

        bool ICollection<T>.IsReadOnly => false;
        
        public PoolList(int capacity = 64)
        {
            _items = ArrayPool<T>.Shared.Rent(capacity);
            _length = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _length; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (_length == _items.Length)
            {
                var newArray = ArrayPool<T>.Shared.Rent(_items.Length * 2);
                _items.CopyTo(newArray, 0);
                ArrayPool<T>.Shared.Return(_items, true);
                _items = newArray;
            }
            _items[_length++] = item;
        }
        
        public void Insert(int index, T item)
        {
            if ((uint) index > (uint) _length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (_length == _items.Length)
            {
                var newArray = ArrayPool<T>.Shared.Rent(_items.Length * 2);
                _items.CopyTo(newArray, 0);
                ArrayPool<T>.Shared.Return(_items, true);
                _items = newArray;
            }
            Array.Copy(_items, index, _items, index + 1, (_length - index));
            _items[index] = item;
        }

        public void RemoveAt(int index)
        {
            if ((uint) index >= (uint) _length)
                throw new ArgumentOutOfRangeException(nameof(index));
            Array.Copy(_items, index + 1, _items, index, (_length - index) - 1);
            _length -= 1;
        }
        
        public bool Remove(T item)
        {
            for (var i = 0; i < _length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        
        public void Clear()
        {
            _length = 0;
        }

        public bool Contains(T item)
        {
            for (var i = 0; i < _length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return true;
            }
            return false;
        }
        
        public int IndexOf(T item)
        {
            for (var i = 0; i < _length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            _items.Slice(0, _length).CopyTo(array.Slice(arrayIndex));
        }

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(_items, true);
            _items = Array.Empty<T>();
            _length = 0;
        }
    }
}