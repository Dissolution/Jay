using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jay.Debugging.Dumping;

namespace Jay.Comparison
{
    public abstract class ComparerBase<T, TSelf> : IComparer<T>,
                                                   IComparer
        where TSelf : ComparerBase<T, TSelf>, new()
    {
        public static TSelf Instance { get; } = new TSelf();

        protected ComparerBase()
        {
            
        }

        /// <inheritdoc />
        int IComparer.Compare(object? left, object? right)
        {
            if (left is T leftT)
            {
                if (right is T rightT)
                {
                    return this.Compare(leftT, rightT);
                }
                return 1;
            }
            else
            {
                if (right is T)
                {
                    return -1;
                }
                return 0;
            }
        }
        
        
        
        /// <inheritdoc />
        int IComparer<T>.Compare(T? left, T? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return this.Compare(left, right);
                }
                return 1;
            }
            else
            {
                if (right != null)
                {
                    return -1;
                }
                return 0;
            }
        }

        public abstract int Compare([DisallowNull] T left, [DisallowNull] T right);
        
        
        /// <inheritdoc />
        public sealed override bool Equals(object? obj) => false;

        /// <inheritdoc />
        public sealed override int GetHashCode() => GetHashCodeException.Throw(this);

        /// <inheritdoc />
        public override string ToString() => Dumper.Dump(this.GetType());
    }

    public abstract class EqualityComparerBase<T, TSelf> : IEqualityComparer<T>,
                                                           IEqualityComparer
        where TSelf : EqualityComparerBase<T, TSelf>, new()
    {
        public static TSelf Instance { get; } = new TSelf();

        protected EqualityComparerBase()
        {
            
        }
        
        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? left, object? right)
        {
            T? leftT;
            if (left is T)
            {
                leftT = (T) left;
            }
            else if (left is null && typeof(T).CanBeNull())
            {
                leftT = default(T);
            }
            else
            {
                return false;
            }

            T? rightT;
            if (right is T)
            {
                rightT = (T) right;
            }
            else if (right is null && typeof(T).CanBeNull())
            {
                rightT = default(T);
            }
            else
            {
                return false;
            }
            return this.Equals(leftT, rightT);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is T value)
                return this.GetHashCode(value);
            return 0;
        }

        /// <inheritdoc />
        public abstract bool Equals(T? left, T? right);

        /// <inheritdoc />
        public virtual int GetHashCode([AllowNull] T value)
        {
            if (value == null)
            {
                return 0;
            }
            return value.GetHashCode();
        }

        /// <inheritdoc />
        public sealed override bool Equals(object? obj) => false;

        /// <inheritdoc />
        public sealed override int GetHashCode() => GetHashCodeException.Throw(this);

        /// <inheritdoc />
        public override string ToString() => Dumper.Dump(this.GetType());
    }
}