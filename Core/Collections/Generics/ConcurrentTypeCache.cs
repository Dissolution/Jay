using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public class ConcurrentTypeCache<TValue> : ConcurrentDictionary<Type, TValue>
    {
        public ConcurrentTypeCache()
            : base(Environment.ProcessorCount, 31) { }
        
        public ConcurrentTypeCache(int capacity)
            : base(Environment.ProcessorCount, capacity) { }

        public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

        public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value) => base.TryGetValue(typeof(T), out value);

        public TValue GetOrAdd<T>(TValue value) => base.GetOrAdd(typeof(T), value);
        public TValue GetOrAdd<T>(Func<TValue> valueFactory) => base.GetOrAdd(typeof(T), _ => valueFactory());
        public TValue GetOrAdd<T>(Func<Type, TValue> valueFactory) => base.GetOrAdd(typeof(T), valueFactory);
    }
}