using System;
using System.Collections.Generic;

namespace Jay.Collections
{
    internal sealed class MultiArrayEnumerator : ArrayEnumerator
    {
        private readonly (int Min, int Max)[] _arrayIndexes;
         
        private int _offset;
        private int[] _indexes;

        public int Dimensions => _array.Rank;
        public int Length => _array.Length;
        
        public int Offset => _offset;
        public IReadOnlyList<int> Indexes => _indexes;

        public MultiArrayEnumerator(Array array)
            : base(array)
        {
            _arrayIndexes = new (int Min, int Max)[Dimensions];
            for (var d = 0; d < Dimensions; d++)
            {
                _arrayIndexes[d] = (_array.GetLowerBound(d), _array.GetUpperBound(d));
            }
            _current = null;
            _offset = -1;
            _indexes = Array.Empty<int>();
        }

        /// <inheritdoc />
        public override bool MoveNext()
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

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            _offset = -1;
            _indexes = Array.Empty<int>();
        }
    }
}