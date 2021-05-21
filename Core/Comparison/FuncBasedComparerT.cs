using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    internal sealed class FuncBasedComparer<T> : IComparer<T>,
                                                 IComparer
    {
        private readonly Func<T?, T?, int> _compare;

        public FuncBasedComparer(Func<T?, T?, int> compare)
        {
            _compare = compare ?? throw new ArgumentNullException(nameof(compare));
        }

        int IComparer.Compare(object? x, object? y)
        {
            if (x is T tx && y is T ty)
                return _compare(tx, ty);
            return 0;
        }
        
        public int Compare(T? x, T? y) => _compare(x, y);
    }
}