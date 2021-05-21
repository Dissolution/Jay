using System;

namespace Jay.Collections.Pools
{
    public static class Pool
    {
        internal static readonly int DefaultCapacity = Environment.ProcessorCount * 2;
        internal const int MaxCapacity = 0X7FEFFFFF; // Array.MaxArrayLength

        public static ObjectPool<T> Create<T>(Func<T> factory,
                                              Action<T>? clean = null,
                                              Action<T>? dispose = null)
            where T : class
            => new ObjectPool<T>(factory, clean, dispose);
        
        public static ObjectPool<T> Create<T>(Action<T>? clean = null,
                                              Action<T>? dispose = null)
            where T : class, new() 
            => new ObjectPool<T>(() => new T(), clean, dispose);

        public static ObjectPool<T> Create<T>(Action<T>? clean = null)
            where T : class, IDisposable, new()
            => new ObjectPool<T>(() => new T(), clean, value => value.Dispose());
        
        public static ObjectPool<T> Create<T>(Func<T> factory,
                                              Action<T>? clean = null)
            where T : class, IDisposable
            => new ObjectPool<T>(factory, clean, value => value.Dispose());
    }
}