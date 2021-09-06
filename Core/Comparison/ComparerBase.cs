using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Comparison
{
    public abstract class ComparerBase<T, TSelf> : IComparer<T>, IComparer
        where TSelf : ComparerBase<T, TSelf>, new()
    {
        public static TSelf Default { get; } = new TSelf();

        protected ComparerBase() : base()
        {

        }

        protected abstract int CompareImpl([DisallowNull] T x, [DisallowNull] T y);
    
        /// <inheritdoc />
        public int Compare(T? x, T? y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;
            return CompareImpl(x, y);
        }

        /// <inheritdoc />
        int IComparer.Compare(object? x, object? y)
        {
            if (x is T xTyped)
            {
                if (y is T yTyped)
                {
                    return CompareImpl(xTyped, yTyped);
                }
                return 1;
            }
            if (y is T) return -1;
            return 0;
        }
    }
}