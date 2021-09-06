using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Reflection.Comparison
{
    public sealed class TypeEqualityComparer : IEqualityComparer<Type?[]>,
                                               IEqualityComparer<Type?>,
                                               IEqualityComparer
    {
        public static TypeEqualityComparer Instance { get; } = new TypeEqualityComparer();
        
        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x is Type[] xTypes)
            {
                if (y is Type[] yTypes)
                {
                    return Equals(xTypes, yTypes);
                }
                if (y is Type yType && xTypes.Length == 1)
                {
                    return yType == xTypes[0];
                }
                return false;
            }
            if (x is Type xType)
            {
                if (y is Type yType)
                {
                    return yType == xType;
                }
                if (y is Type[] yTypes && yTypes.Length == 1)
                {
                    return yTypes[0] == xType;
                }
                return false;
            }
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is Type[] types) return Hasher.Create<Type>(types);
            if (obj is Type type) return Hasher.Create<Type>(type);
            return 0;
        }
        
        /// <inheritdoc />
        public bool Equals(Type?[]? x, Type?[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            var len = x.Length;
            if (y.Length != len) return false;
            for (var i = 0; i < len; i++)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(Type?[]? types)
        {
            return Hasher.Create<Type>(types);
        }

        /// <inheritdoc />
        public bool Equals(Type? x, Type? y)
        {
            return x == y;
        }

        /// <inheritdoc />
        public int GetHashCode(Type? type)
        {
            return Hasher.Create<Type>(type);
        }

     
    }
}