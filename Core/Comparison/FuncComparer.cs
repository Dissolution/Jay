using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public sealed class FuncComparer<T> : IComparer<T>, IComparer
    {
        private readonly Func<T?, T?, int> _compare;

        public FuncComparer(Func<T?, T?, int> compare)
        {
            _compare = compare ?? throw new ArgumentNullException(nameof(compare));
        }

        /// <inheritdoc />
        public int Compare(T? x, T? y)
        {
            return _compare(x, y);
        }

        /// <inheritdoc />
        int IComparer.Compare(object? x, object? y)
        {
            if (x is T xTyped)
            {
                if (y is T yTyped)
                {
                    return _compare(xTyped, yTyped);
                }
                return 1;
            }
            if (y is T) return -1;
            return 0;
        }
    }
}