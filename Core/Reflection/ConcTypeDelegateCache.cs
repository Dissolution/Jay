using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jay.Collections;

namespace Jay.Reflection
{
    public abstract class TypeDelegateCache
    {
        protected readonly ConcurrentTypeDictionary<Delegate> _cache;
        protected readonly ConcurrentTypeDictionary<Delegate> _objCache;

        public TypeDelegateCache()
        {
            _cache = new();
            _objCache = new();
        }

        public virtual void ValidateDelegate<TDelegate>()
            where TDelegate : Delegate
        {
            ValidateDelegate(typeof(TDelegate));
        }

        public virtual void ValidateDelegate(Type delegateType)
        {
            ArgumentNullException.ThrowIfNull(delegateType);
            if (!delegateType.Implements<Delegate>())
                throw new ArgumentException("", nameof(delegateType));
        }

        public abstract Delegate CreateDelegate(Type type);

        
        public virtual TDelegate GetDelegate<TDelegate>(Type type)
            where TDelegate : Delegate
        {
            ValidateDelegate<TDelegate>();
            var cachedDelegate = _cache.GetOrAdd(type, CreateDelegate);
            if (cachedDelegate is TDelegate tDelegate)
                return tDelegate;
            throw new InvalidOperationException();
        }
    }
}
