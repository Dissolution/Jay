using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    internal class TypeCache<TValue> : Dictionary<Type, TValue>
    {
        public TypeCache(int capacity = 31)
            : base(capacity) { }

        public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

        public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value) => base.TryGetValue(typeof(T), out value);

        public TValue GetOrAdd<T>(TValue value)
        {
            if (base.TryGetValue(typeof(T), out TValue? found))
                return found;
            base.Add(typeof(T), value);
            return value;
        }

        public TValue GetOrAdd<T>(Func<TValue> valueFactory)
        {
            if (!base.TryGetValue(typeof(T), out TValue? found))
            {
                found = valueFactory();
                base.Add(typeof(T), found);
            }
            return found;
        }
        
        public TValue GetOrAdd<T>(Func<Type, TValue> valueFactory)
        {
            if (!base.TryGetValue(typeof(T), out TValue? found))
            {
                found = valueFactory(typeof(T));
                base.Add(typeof(T), found);
            }
            return found;
        }
    }
    
    internal class ConcurrentTypeCache<TValue> : ConcurrentDictionary<Type, TValue>
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