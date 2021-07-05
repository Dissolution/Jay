using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Corvidae
{
    internal sealed class ValueSources : IEnumerable<IValueSource>
    {
        private readonly ConcurrentDictionary<Type, IValueSource> _cache;

        public IEnumerable<ISingletonSource> Singletons => _cache.Values.OfType<ISingletonSource>();
        
        public ValueSources()
        {
            _cache = new ConcurrentDictionary<Type, IValueSource>();
        }

        public bool Contains(Type type) => _cache.ContainsKey(type);
        
        public bool Contains<TValue>() => _cache.ContainsKey(typeof(TValue));

        public bool TryAdd<TValue>(IValueSource<TValue> valueSource)
        {
            return _cache.TryAdd(typeof(TValue), valueSource);
        }
        
        public IValueSource<TValue> GetOrAdd<TValue>(Func<Type, IValueSource> valueSourceFactory)
        {
            return (_cache.GetOrAdd(typeof(TValue), valueSourceFactory) as IValueSource<TValue>)!;
        }

        internal void Set(Type type, IValueSource valueSource)
        {
            _cache[type] = valueSource;
        }
        
        public void Set<TValue>(IValueSource<TValue> valueSource)
        {
            _cache[typeof(TValue)] = valueSource;
        }

        /// <inheritdoc />
        public IEnumerator<IValueSource> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}