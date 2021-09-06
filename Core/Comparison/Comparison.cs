using Jay.Collections;
using Jay.Debugging;
using Jay.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using Jay.Reflection.Emission;

namespace Jay.Comparison
{
    public static class Comparison
    {
        private static readonly ConcurrentTypeCache<IEqualityComparer> _equalityComparerCache;
        private static readonly ConcurrentTypeCache<IComparer> _comparerCache;

        static Comparison()
        {
            _equalityComparerCache = new ConcurrentTypeCache<IEqualityComparer>();
            _comparerCache = new ConcurrentTypeCache<IComparer>();
        }

        private static IEqualityComparer GetEqualityComparerForType(Type type)
        {
            return (typeof(EqualityComparer<>)
                    .MakeGenericType(type)
                    .GetProperty(nameof(EqualityComparer<DBNull>.Default), Reflect.StaticFlags)?
                    .GetValue(null) as IEqualityComparer)
                .ThrowIfNull();
        }

        private static IComparer GetComparerForType(Type type)
        {
            return (typeof(Comparer<>)
                    .MakeGenericType(type)
                    .GetProperty(nameof(Comparer<DBNull>.Default), Reflect.StaticFlags)?
                    .GetValue(null) as IComparer)
                .ThrowIfNull();
        }
        
        public static IEqualityComparer<T> DefaultEqualityComparer<T>() => EqualityComparer<T>.Default;

        public static IEqualityComparer DefaultEqualityComparer(Type? type)
        {
            return _equalityComparerCache.GetOrAdd(type ?? typeof(object), GetEqualityComparerForType);
        }

        public static IComparer<T> DefaultComparer<T>() => Comparer<T>.Default;

        public static IComparer DefaultComparer(Type? type)
        {
            return _comparerCache.GetOrAdd(type ?? typeof(object), GetComparerForType);
        }

        public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals,
                                                                     Func<T?, int> getHashCode)
        {
            return new FuncEqualityComparer<T>(equals, getHashCode);
        }
        
        public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        {
            return new FuncComparer<T>(compare);
        }

        new public static bool Equals(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            var xType = x.GetType();
            if (y.GetType() != xType) return false;
            return DefaultEqualityComparer(xType).Equals(x, y);
        }

        public static int Compare(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            var xType = x.GetType();
            if (y.GetType() != xType) return 0;
            return DefaultComparer(xType).Compare(x, y);
        }
    }
}