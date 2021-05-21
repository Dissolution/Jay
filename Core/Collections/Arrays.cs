using Jay.Comparison;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    public static class Arrays
    {
        public static ArrayEnumerator GetEnumerator(this Array array) => new ArrayEnumerator(array);
        public static ArrayEnumerable AsArrayEnumerable(this Array array) => new ArrayEnumerable(array);
    }

    public class ArrayEnumerable : IEnumerable<object?>, IEnumerable
    {
        private readonly Array _array;

        public ArrayEnumerable(Array array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
        }
        
        public ArrayEnumerator GetEnumerator()
        {
            return new ArrayEnumerator(_array);
        }
        
        IEnumerator<object?> IEnumerable<object?>.GetEnumerator()
        {
            return new ArrayEnumerator(_array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ArrayEnumerator(_array);
        }
    }
    
    public class ArrayEnumerator : IEnumerator<object?>, IEnumerator, IDisposable
    {
        protected readonly Array _array;
        protected readonly (int Min, int Max)[] _arrayIndexes;
        
        protected object? _current;
        protected int _offset;
        protected int[] _indexes;

        public int Dimensions => _array.Rank;
        public int Length => _array.Length;

        public object? Current => _current;
        public int Offset => _offset;
        public IReadOnlyList<int> Indexes => _indexes;
        
        public ArrayEnumerator(Array array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
            _arrayIndexes = new (int Min, int Max)[Dimensions];
            for (var d = 0; d < Dimensions; d++)
            {
                _arrayIndexes[d] = (_array.GetLowerBound(d), _array.GetUpperBound(d));
            }
            _current = null;
            _offset = -1;
            _indexes = Array.Empty<int>();
        }

        public bool MoveNext()
        {
            var nextOffset = _offset + 1;
            if (nextOffset > Length)
                return false;
            ref int[] idx = ref _indexes;
            if (nextOffset == 0)
            {
                idx = new int[Dimensions];
                for (var d = 0; d < Dimensions; d++)
                {
                    idx[d] = _arrayIndexes[d].Min;
                }
            }
            else
            {
                // Rotate
                for (var d = Dimensions - 1; d >= 0; d--)
                {
                    var (min, max) = _arrayIndexes[d];
                    if (idx[d] < max)
                    {
                        idx[d]++;
                        break;
                    }
                    else
                    {
                        idx[d] = min;
                    }
                }
            }

            _current = _array.GetValue(_indexes);
            _offset = nextOffset;
            return true;
        }

        public void Reset()
        {
            _current = null;
            _offset = -1;
            _indexes = Array.Empty<int>();
        }

        void IDisposable.Dispose()
        {
            // Do nothing, just for ease-of-use with 'using'
        }
    }
}