using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jay.Collections;
using Microsoft.Extensions.Logging;

namespace Corvidae
{
    public class NestRegistration
    {
        internal readonly ValueSources _valueSources;
        internal readonly List<IAcquisitionErrorHandler> _errorHandlers;
        internal readonly List<ILogger<ISource>> _loggers;
        
        public IData Data { get; }

        public NestRegistration()
        {
            _valueSources = new ValueSources();
            _errorHandlers = new List<IAcquisitionErrorHandler>(1);
            _loggers = new List<ILogger<ISource>>(1);
            this.Data = new Data();
        }
        
        public NestRegistration AddSingleton<T>(T? singleton)
        {
            _valueSources.Set<T>(new SingletonSource<T>(singleton));
            return this;
        }
        
        public NestRegistration AddSingleton<T>(Func<T?> singletonFactory)
        {
            _valueSources.Set<T>(new SingletonSource<T>(singletonFactory));
            return this;
        }
        
        public NestRegistration AddSingleton<T>(Func<ISource, T?> singletonFactory)
        {
            _valueSources.Set<T>(new DeferredSingletonSource<T>(singletonFactory));
            return this;
        }
        
        public NestRegistration AddSingleton<TImplementation, TSingleton>(TSingleton? singleton) 
            where TSingleton : TImplementation
        {
            _valueSources.Set(typeof(TImplementation), new SingletonSource<TSingleton>(singleton));
            return this;
        }
        
        public NestRegistration AddSingleton<TImplementation, TSingleton>(Func<TSingleton?> singletonFactory) 
            where TSingleton : TImplementation
        {
            _valueSources.Set(typeof(TImplementation), new SingletonSource<TSingleton>(singletonFactory));
            return this;
        }
        
        public NestRegistration AddSingleton<TImplementation, TSingleton>(Func<ISource, TSingleton?> singletonFactory) 
            where TSingleton : TImplementation
        {
            _valueSources.Set(typeof(TImplementation), new DeferredSingletonSource<TSingleton>(singletonFactory));
            return this;
        }
        
        public NestRegistration AddTransient<T>(Func<T?> transientFactory)
        {
            _valueSources.Set<T>(FactorySource<T>.Create(transientFactory));
            return this;
        }
        
        public NestRegistration AddTransient<T>(Func<ISource, T?> transientFactory)
        {
            _valueSources.Set<T>(FactorySource<T>.Create(transientFactory));
            return this;
        }
        
        public NestRegistration AddErrorHandler(IAcquisitionErrorHandler errorHandler)
        {
            _errorHandlers.Add(errorHandler);
            return this;
        }
        
        public NestRegistration AddLogger(ILogger<ISource> logger)
        {
            _loggers.Add(logger);
            return this;
        }
        //
        // public SourceRegistration Scan(Assembly assembly, Action<SourceRegistration, Type> perTypeRegistration)
        // {
        //     var assemblyTypes = assembly.GetExportedTypes();
        //     for (var i = 0; i < assemblyTypes.Length; i++)
        //     {
        //         perTypeRegistration(this, assemblyTypes[i]);
        //     }
        //     return this;
        // }
    }
}