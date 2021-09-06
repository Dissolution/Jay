using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public class TypeCache<TValue> : Dictionary<Type, TValue>
    {
        public TypeCache()
            : base() { }
        public TypeCache(int capacity)
            : base(capacity) { }
        
        public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

        public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value) => base.TryGetValue(typeof(T), out value);

        public TValue GetOrAdd<T>(TValue value)
        {
            if (base.TryGetValue(typeof(T), out var existingValue))
            {
                return existingValue;
            }
            return (base[typeof(T)] = value);
        }

        public TValue GetOrAdd<T>(Func<TValue> valueFactory)
        {
            if (base.TryGetValue(typeof(T), out var existingValue))
            {
                return existingValue;
            }
            return (base[typeof(T)] = valueFactory());
        }
        public TValue GetOrAdd<T>(Func<Type, TValue> valueFactory)
        {
            if (base.TryGetValue(typeof(T), out var existingValue))
            {
                return existingValue;
            }
            return (base[typeof(T)] = valueFactory(typeof(T)));
        }
    }
}