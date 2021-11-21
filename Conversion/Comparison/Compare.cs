using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conversion.Comparison
{
    

    public interface ISpanEqualityComparer<T>
    {
        bool Equals(Span<T> x, Span<T> y);

        int GetHashCode(Span<T> span);
    }

    public interface IReadOnlySpanEqualityComparer<T>
    {
        bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y);

        int GetHashCode(ReadOnlySpan<T> span);
    }

    public class EnumerableEqualityComparer<T> : IEqualityComparer<T>,
                                                 IEqualityComparer<T[]>,
                                                 IEqualityComparer<IEnumerable<T>>,
                                                 IReadOnlySpanEqualityComparer<T>
    {
        public static EnumerableEqualityComparer<T> Default { get; } =
            new EnumerableEqualityComparer<T>(EqualityComparer<T>.Default);

        protected readonly IEqualityComparer<T> _itemEqualityComparer;

        public EnumerableEqualityComparer(IEqualityComparer<T>? itemEqualityComparer = null)
        {
            _itemEqualityComparer = itemEqualityComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(T? x, T? y) => _itemEqualityComparer.Equals(x, y);

        public int GetHashCode(T? value)
        {
            if (value is null) return 0;
            return _itemEqualityComparer.GetHashCode(value);
        }

        public bool Equals(T[]? x, T[]? y)
        {
            if (x is null) return y is null;
            if (y is null) return x is null;
            var xLen = x.Length;
            if (y.Length != xLen) return false;
            for (var i = 0; i < xLen; i++)
            {
                if (!Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        public int GetHashCode(T[]? array)
        {
            if (array is null) return 0;
            var hash = new HashCode();
            for (var i = 0; i < array.Length; i++)
            {
                hash.Add(array[i], _itemEqualityComparer);
            }
            return hash.ToHashCode();
        }

        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if (x is null) return y is null;
            if (y is null) return x is null;
            using (var xe = x.GetEnumerator())
            using (var ye = y.GetEnumerator())
            {
                var xEnd = xe.MoveNext();
                var yEnd = ye.MoveNext();
                while (xEnd == yEnd)
                {
                    if (xEnd == false && yEnd == false) return true;
                    if (!Equals(xe.Current, ye.Current))
                        return false;
                    xEnd = xe.MoveNext();
                    yEnd = ye.MoveNext();
                }
                return false;
            }
        }

        public int GetHashCode(IEnumerable<T>? values)
        {
            if (values is null) return 0;
            var hash = new HashCode();
            foreach (var value in values)
            {
                hash.Add(value, _itemEqualityComparer);
            }
            return hash.ToHashCode();
        }

        public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
        {
            return MemoryExtensions.SequenceEqual<T>(x, y, _itemEqualityComparer);
        }

        public int GetHashCode(ReadOnlySpan<T> span)
        {
            var hash = new HashCode();
            for (var i = 0; i < span.Length; i++)
            {
                hash.Add(span[i], _itemEqualityComparer);
            }
            return hash.ToHashCode();
        }
    }
}
