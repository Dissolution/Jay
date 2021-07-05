using Jay.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public sealed class ArrayEqualityComparer : EqualityComparerBase<Array, ArrayEqualityComparer>
    {
        private readonly IEqualityComparer<object> _equalityComparer;

        public ArrayEqualityComparer()
        {
            _equalityComparer = EqualityComparer<object>.Default;
        }
        
        public ArrayEqualityComparer(IEqualityComparer<object> itemEqualityComparer)
        {
            _equalityComparer = itemEqualityComparer ?? EqualityComparer<object>.Default;
        }

        public override bool Equals(Array? x, Array? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.Length != y.Length) return false;
            using var xe = x.GetArrayEnumerator();
            using var ye = y.GetArrayEnumerator();
            while (xe.MoveNext() && ye.MoveNext())
            {
                if (!_equalityComparer.Equals(xe.Current, ye.Current))
                    return false;
            }
            return true;
        }

        public override int GetHashCode(Array? array)
        {
            if (array is null) return 0;
            var hasher = new Hasher();
            IEnumerator? e = null;
            try
            {
                e = array.GetEnumerator();
                while (e.MoveNext())
                {
                    hasher.Add(e.Current, _equalityComparer);
                }
                return hasher.ToHashCode();
            }
            finally
            {
                Result.Dispose(e);
            }
        }
    }
}