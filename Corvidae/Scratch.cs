/*using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Jay;
using Jay.Collections;
using Jay.Debugging;

namespace Corvidae
{
    public sealed class TypeExactnessComparer : IComparer<Type>
    {
        /// <inheritdoc />
        public int Compare(Type? x, Type? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            var xInfo = (x.IsValueType, x.IsClass, x.IsInterface, x.IsGenericType, x.IsGenericTypeDefinition);
            var yInfo = (x.IsValueType, x.IsClass, x.IsInterface, x.IsGenericType, x.IsGenericTypeDefinition);
            Hold.Debug(xInfo, yInfo);
            return 0;
        }
    }
    
    [Flags]
    public enum ImplementTypes
    {
        None = 0,
        Exact = 1 << 0,
        Interfaces = 1 << 1,
        BaseClasses = 1 << 2,
        Generic = 1 << 3,
    }

    internal sealed class SingletonCache
    {
        private readonly Dictionary<Type, object> _values;

        public SingletonCache()
        {
            _values = new Dictionary<Type, object>();
        }

        public bool TryAdd<T>([DisallowNull] T value)
        {
            return _values.TryAdd(typeof(T), value);
        }

        internal void Set(Type type, object value)
        {
            _values[type] = value;
        }
        
        public void Set<T>([DisallowNull] T value)
        {
            _values[typeof(T)] = value;
        }
        
        public bool Contains<T>() => _values.ContainsKey(typeof(T));
        public bool TryGetValue<T>([NotNullWhen(true)] out T value)
        {
            if (_values.TryGetValue(typeof(T), out var obj))
            {
                return obj!.Is(out value!);
            }
            value = default!;
            return false;
        }

        [return: NotNull]
        public T GetOrAdd<T>([DisallowNull] T value)
        {
            if (TryGetValue<T>(out var existingValue))
                return existingValue;
            _values[typeof(T)] = value;
            return value;
        }
        
        [return: NotNull]
        public T GetOrAdd<T>(Func<T> valueFactory)
        {
            if (TryGetValue<T>(out var existingValue))
                return existingValue;
            existingValue = valueFactory();
            if (existingValue is null)
                throw new ArgumentException("Null may not be stored as a singleton", nameof(valueFactory));
            _values[typeof(T)] = existingValue;
            return existingValue;
        }
    }
    
    public sealed class Source : ISource
    {
        internal IEnumerable<Type> GetImplementationMatchTypes(Type type, ImplementTypes imt)
        {
            if (imt == ImplementTypes.None)
                yield break;
            if (imt.HasTheFlag(ImplementTypes.Exact))
                yield return type;
            if (imt.HasTheFlag(ImplementTypes.BaseClasses))
            {
                Type? baseType = type.BaseType;
                while (baseType != null && baseType != typeof(object))
                {
                    yield return baseType;
                    baseType = baseType.BaseType;
                }
            }
            if (imt.HasTheFlag(ImplementTypes.Interfaces))
            {
                var interfaces = type.GetInterfaces();
                foreach (var interfac in interfaces)
                {
                    yield return interfac;
                }
            }

            if (imt.HasTheFlag(ImplementTypes.Generic))
            {
                Debugger.Break();
            }
        }
        
        public static ISource Create() => throw new NotImplementedException();


        private readonly SingletonCache _singletons;
        private readonly TypeCache<Lazy<object>> _perRequests;
        
        public Source()
        {
            _singletons = new SingletonCache();
            _perRequests = new TypeCache<Lazy<object>>();
        }


        /// <inheritdoc />
        public ISource AddSingleton<T>([DisallowNull] T singleton,
                                       ImplementTypes implementTypes = ImplementTypes.Exact)
        {
            foreach (var type in GetImplementationMatchTypes(typeof(T), implementTypes))
            {
                _singletons.Set(type, singleton!);
            }
            return this;
        }

        /// <inheritdoc />
        public ISource AddSingleton<T>(Func<T> singletonFactory, ImplementTypes implementTypes = ImplementTypes.Exact)
        {
            var singleton = singletonFactory();
            if (singleton is null)
                throw new ArgumentException("Null may not be stored as a singleton", nameof(singletonFactory));
            foreach (var type in GetImplementationMatchTypes(typeof(T), implementTypes))
            {
                _singletons.Set(type, singleton);
            }
            return this;
        }

        /// <inheritdoc />
        public ISource AddPerRequest<T>(Func<T> valueFactory, ImplementTypes implementTypes = ImplementTypes.Exact)
        {
            foreach (var type in GetImplementationMatchTypes(typeof(T), implementTypes))
            {
                _perRequests[type] = new Lazy<object>(valueFactory);
            }
            return this;
        }

        /// <inheritdoc />
        [return: NotNull]
        public T Get<T>()
        {
            if (_singletons.TryGetValue<T>(out var value))
            {
                return value;
            }

            // Now we abuse
            var factory = _perRequests.GetOrAdd<T>(ValueConstructor);
            return factory.Value.As<T>();
        }

        private Lazy<object> ValueConstructor(Type type)
        {
            throw new NotImplementedException();
        }
    }
    
    public interface ISource
    {
        ISource AddSingleton<T>([DisallowNull] T singleton,
                                ImplementTypes implementTypes = ImplementTypes.Exact);

        ISource AddSingleton<T>(Func<T> singletonFactory,
                                ImplementTypes implementTypes = ImplementTypes.Exact);

        ISource AddPerRequest<T>(Func<T> valueFactory,
                                 ImplementTypes implementTypes = ImplementTypes.Exact);

        [return: NotNull]
        T Get<T>();

    }
}*/