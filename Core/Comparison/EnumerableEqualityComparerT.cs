using System;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public class EnumerableEqualityComparer<T> : IEqualityComparer<T[]>,
                                                 IEqualityComparer<IEnumerable<T>>
    {
        public static EnumerableEqualityComparer<T> Instance { get; } = new EnumerableEqualityComparer<T>(null);
        
        private readonly IEqualityComparer<T>? _equalityComparer;

        public EnumerableEqualityComparer(IEqualityComparer<T>? itemEqualityComparer)
        {
            _equalityComparer = itemEqualityComparer;
        }

        public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
        {
            if (x.Length != y.Length)
                return false;
            if (_equalityComparer is null)
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (!EqualityComparer<T>.Default.Equals(x[i], y[i]))
                        return false;
                }
                return true;
            }
            else
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (!_equalityComparer.Equals(x[i], y[i]))
                        return false;
                }
                return true;
            }
        }

        public int GetHashCode(ReadOnlySpan<T> span)
        {
            return Hasher.Create<T>(span, _equalityComparer);
        }

        public bool Equals(T[]? x, T[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.Length != y.Length)
                return false;
            if (_equalityComparer is null)
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (!EqualityComparer<T>.Default.Equals(x[i], y[i]))
                        return false;
                }
                return true;
            }
            else
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (!_equalityComparer.Equals(x[i], y[i]))
                        return false;
                }
                return true;
            }
        }

        public int GetHashCode(T[]? array)
        {
            return Hasher.Create<T>(array, _equalityComparer);
        }

        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            IEqualityComparer<T> eq = _equalityComparer ?? EqualityComparer<T>.Default;
            using (var xe = x.GetEnumerator())
            using (var ye = y.GetEnumerator())
            {
                while (true)
                {
                    bool xMoved = xe.MoveNext();
                    bool yMoved = ye.MoveNext();
                    if (xMoved != yMoved)
                        return false;
                    if (!xMoved)
                        return true;
                    if (!eq.Equals(xe.Current, ye.Current))
                        return false;
                }
            }
        }

        public int GetHashCode(IEnumerable<T>? values)
        {
            if (values is null) return 0;
            var hasher = new Hasher();
            foreach (var value in values)
            {
                hasher.Add(value, _equalityComparer);
            }
            return hasher.ToHashCode();
        }
    }
}