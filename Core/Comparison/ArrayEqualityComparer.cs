using Jay.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public class ArrayEqualityComparer : IEqualityComparer<Array>
    {
        private readonly IEqualityComparer<object> _equalityComparer;

        public ArrayEqualityComparer(IEqualityComparer<object>? itemEqualityComparer = null)
        {
            _equalityComparer = itemEqualityComparer ?? EqualityComparer<object>.Default;
        }

        public bool Equals(Array? x, Array? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.Length != y.Length) return false;
            var xe = Arrays.GetEnumerator(x);
            var ye = Arrays.GetEnumerator(y);
            while (xe.MoveNext() && ye.MoveNext())
            {
                if (!_equalityComparer.Equals(xe.Current, ye.Current))
                    return false;
            }
            return true;
        }

        public int GetHashCode(Array? array)
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