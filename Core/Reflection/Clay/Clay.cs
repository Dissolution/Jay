/*using Jay.Reflection.Duck;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection
{
    public static class Clay
    {
        private const string AssemblyName = "Clay_Assembly";
        private const string ModuleName = "Clay_Module";
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly ModuleBuilder _moduleBuilder;
        
        private static readonly ConcurrentDictionary<(Type SourceType, Type DestType), Type> _createdTypes;

        public static ModuleBuilder ModuleBuilder => _moduleBuilder;
        
        static Clay()
        {
            _createdTypes = new ConcurrentDictionary<(Type SourceType, Type DestType), Type>();
            
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName),
                                                                     AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(ModuleName);
        }

        /// <summary>
        /// Make the specified object mimic the specified Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ProxyAs<T>(object value)
        { 
            if (value is T)
                return (T)value;
            if (!typeof(T).IsInterface)
                throw new InvalidOperationException("You can only ProxyAx interfaces");

            return CreateProxy<T>(value);
        }

        private static T CreateProxy<T>(object value)
        {
            var sourceType = value?.GetType();
            var destType = typeof(T);
            var proxyKey = (sourceType, destType);
            if (!_createdTypes.TryGetValue(proxyKey, out var proxyType))
            {
                var factory = new ProxyFactory(sourceType, destType);
                proxyType = factory.CreateProxyType();
                _createdTypes.TryAdd(proxyKey, proxyType);
            }

            var instance = (T)Activator.CreateInstance(proxyType, value);
            return instance;
        }
    }
}*/


namespace Jay.Reflection
{
    public static partial class Clay
    {
    
    }
}