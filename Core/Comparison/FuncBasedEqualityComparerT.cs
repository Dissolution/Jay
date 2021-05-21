using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jay.Comparison
{
    internal sealed class FuncBasedEqualityComparer<T> : IEqualityComparer<T>,
                                                         IEqualityComparer
    {
        private readonly Func<T?, T?, bool> _equals;
        private readonly Func<T?, int> _getHashCode;

        public FuncBasedEqualityComparer(Func<T?, T?, bool> equals,
                                         Func<T?, int> getHashCode)
        {
            _equals = @equals ?? throw new ArgumentNullException(nameof(equals));
            _getHashCode = getHashCode ?? throw new ArgumentNullException(nameof(getHashCode));
        }

        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x is T tx && y is T ty)
                return _equals(tx, ty);
            return false;
        }

        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is T value)
                return _getHashCode(value);
            return 0;
        }
        
        public bool Equals(T? x, T? y) => _equals(x, y);

        public int GetHashCode(T? value) => _getHashCode(value);
    }
}