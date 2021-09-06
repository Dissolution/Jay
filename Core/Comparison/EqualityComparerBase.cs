using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jay.Debugging.Dumping;
using Jay.Exceptions;

namespace Jay.Comparison
{
    public abstract class EqualityComparerBase<T, TSelf> : IEqualityComparer<T>, IEqualityComparer
        where TSelf : EqualityComparerBase<T, TSelf>, new()
    {
        public static TSelf Default { get; } = new TSelf();

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

        protected abstract bool EqualsImpl([DisallowNull] T left, [DisallowNull] T right);

        /// <inheritdoc />
        public virtual bool Equals(T? left, T? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return this.EqualsImpl(left, right);
                }
                return false;
            }
            return right is null;
        }

        /// <inheritdoc />
        public virtual int GetHashCode(T? value)
        {
            if (value is null)
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