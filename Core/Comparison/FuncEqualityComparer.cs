using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public sealed class FuncEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        private static int DefaultGetHashCode(T? value) => value is null ? 0 : value.GetHashCode();
    
        private readonly Func<T?, T?, bool> _equals;
        private readonly Func<T?, int> _getHashCode;

        public FuncEqualityComparer(Func<T?, T?, bool> equals,
                                    Func<T?, int>? getHashCode = null)
        {
            _equals = equals;
            _getHashCode = getHashCode ?? DefaultGetHashCode;
        }

        /// <inheritdoc />
        public bool Equals(T? x, T? y)
        {
            return _equals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(T? value)
        {
            return _getHashCode(value);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x is T xTyped && y is T yTyped) return _equals(xTyped, yTyped);
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is T t) return _getHashCode(t);
            return 0;
        }
    }
}