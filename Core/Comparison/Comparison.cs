using Jay.Collections;
using Jay.Debugging;
using Jay.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Comparison
{
    public static class Comparison
    {
        private static readonly ConcurrentTypeCache<IEqualityComparer> _equalityCache;

        static Comparison()
        {
            _equalityCache = new ConcurrentTypeCache<IEqualityComparer>();
        }

        private static IEqualityComparer GetEqualityComparerForType(Type type)
        {
            return (typeof(EqualityComparer<>)
                    .MakeGenericType(type)
                    .GetProperty("Default", Reflect.StaticFlags)?
                    .GetValue(null) as IEqualityComparer)
                .ThrowIfNull();
        }
        
        public static IEqualityComparer<T> DefaultEqualityComparer<T>() => EqualityComparer<T>.Default;

        public static IEqualityComparer DefaultEqualityComparer(Type? type)
        {
            return _equalityCache.GetOrAdd(type ?? typeof(object), GetEqualityComparerForType);
        }

        public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals,
                                                                     Func<T?, int> getHashCode)
        {
            return new FuncBasedEqualityComparer<T>(equals, getHashCode);
        }
        
        public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        {
            return new FuncBasedComparer<T>(compare);
        }

        public new static bool Equals(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            var type = x.GetType();
            if (y.GetType() != type) return false;
            return DefaultEqualityComparer(type).Equals(x, y);
        }
      
    }
}