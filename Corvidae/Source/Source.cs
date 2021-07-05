using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Jay;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Reflection;
using Jay.Reflection.Comparison;
using Jay.Reflection.Runtime;
using Microsoft.Extensions.Logging;

namespace Corvidae
{
    public class Source : ISource
    {
        private readonly IReadOnlyList<ILogger<ISource>> _loggers;
        private readonly IReadOnlyList<IAcquisitionErrorHandler> _errorHandlers;
        private readonly ValueSources _valueSources;
        
        /// <inheritdoc />
        public IReadOnlyData Data { get; }

        public Source(NestRegistration registration)
        {
            this.Data = registration.Data;
            _loggers = registration._loggers.AsReadOnly();
            _errorHandlers = registration._errorHandlers.AsReadOnly();
            _valueSources = registration._valueSources;
        }

        private static readonly MethodInfo _acquireMethod = typeof(Source)
                                                            .GetMethod(nameof(Get),
                                                                       BindingFlags.Public | BindingFlags.Instance)
                                                            .ThrowIfNull();

        protected void Log(LogLevel logLevel, string message, params object?[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(logLevel, message, args);
            }
        }
        
        protected void Log(LogLevel logLevel, Exception exception, string message, params object?[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(logLevel, exception, message, args);
            }
        }
        
        /// <inheritdoc />
        public TValue? Get<TValue>()
        {
            IValueSource<TValue> valueSource = _valueSources.GetOrAdd<TValue>(type => CreateValueSource<TValue>(type));
            return valueSource.Get(this);
        }

        private static IValueSource<T> CreateConstructorValueSource<T>(ConstructorInfo constructorInfo)
        {
            Debug.Assert(constructorInfo.DeclaringType.Implements<T>());
            var d = DelegateBuilder.Generate<Func<ISource, T>>(dm =>
            {
                var emitter = dm.Emitter;
                foreach (var pi in constructorInfo.GetParameters())
                {
                    emitter.LoadArgument(0)
                           .Call(_acquireMethod.MakeGenericMethod(pi.ParameterType));
                }
                emitter.New(constructorInfo)
                       .Return();
            });
            return new SourcefulFactorySource<T>(d);
        }

        private bool CanConstruct(ConstructorInfo ctor)
        {
            return ctor.GetParameters().All(pi => _valueSources.Contains(pi.ParameterType));
        }
        
        private IValueSource<T> CreateValueSource<T>(Type type)
        {
            // Check if we have an existing Singletons that can implement this
            var singleTon = _valueSources.Singletons.OrderByDescending(ss => ss.ValueType,
                                                                       TypeComplexityComparer.Instance)
                                         .FirstOrDefault();
            if (singleTon is IValueSource<T> vs)
            {
                return vs;
            }
            
            if (type.IsValueType || type.IsClass)
            {
                var constructors = type.GetConstructors(Reflect.InstanceFlags)
                                       .OrderByDescending<ConstructorInfo>(MethodComplexityComparer.Instance);
                foreach (var ctor in constructors)
                {
                    if (CanConstruct(ctor))
                    {
                        return CreateConstructorValueSource<T>(ctor);
                    }
                }

                // Could not find a constructor that we could construct
                Debug.Assert(!type.IsValueType);
                var ex = new InvalidOperationException(Dumper.Format($"Could not construct a {type} value"));
                Log(LogLevel.Critical, ex, "Could not construct a {Type} value", type);
                throw ex;
            }
            else if (type.IsInterface)
            {
                var types = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .SelectMany(assembly => assembly.ExportedTypes)
                                     .Where(t => t.HasInterface(type))
                                     .Where(t => !t.IsAbstract && !t.IsInterface)
                                     .OrderByDescending(TypeComplexityComparer.Instance)
                                     .ToList();
                Hold.Debug(types);
                foreach (var t in types)
                {
                    var constructors = t.GetConstructors(Reflect.InstanceFlags)
                                        .OrderByDescending<ConstructorInfo>(MethodComplexityComparer.Instance);
                    foreach (var ctor in constructors)
                    {
                        if (CanConstruct(ctor))
                        {
                            return CreateConstructorValueSource<T>(ctor);
                        }
                    }
                }
                throw new NotImplementedException();

            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            foreach (var valueSource in _valueSources)
            {
                valueSource.Dispose();
            }
        }
    }
}