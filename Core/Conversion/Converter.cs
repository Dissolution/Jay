using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Reflection;
using Jay.Reflection.Delegates;
using Jay.Reflection.Emission;
using Jay.Reflection.Runtime;
using JetBrains.Annotations;

namespace Jay.Conversion
{
    public static partial class Converter
    {
        private readonly struct ConverterTypes : IEquatable<ConverterTypes>
        {
            public static implicit operator ConverterTypes((Type InputType, Type OutputType) types) =>
                new ConverterTypes(types.InputType, types.OutputType);
            
            public static bool operator ==(ConverterTypes a, ConverterTypes b) => a.Equals(b);
            public static bool operator !=(ConverterTypes a, ConverterTypes b) => !a.Equals(b);
            
            public readonly Type InputType;
            public readonly Type OutputType;

            public ConverterTypes(Type inputType, Type outputType)
            {
                this.InputType = inputType;
                this.OutputType = outputType;
            }

            public void Deconstruct([JetBrains.Annotations.NotNull] out Type inputType, [JetBrains.Annotations.NotNull] out Type outputType)
            {
                inputType = InputType;
                outputType = OutputType;
            }

            /// <inheritdoc />
            public bool Equals(ConverterTypes converterTypes)
            {
                return converterTypes.InputType == this.InputType &&
                       converterTypes.OutputType == this.OutputType;
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                return obj is ConverterTypes converterTypes && Equals(converterTypes);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return Hasher.Create(InputType, OutputType);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return Dumper.Format($"{InputType} -> {OutputType}");
            }
        }
        
        
        private delegate Result TryConvertDelegate<in TIn, TOut>([AllowNull] TIn input, out TOut output, ConvertOptions options = default);
        private static readonly ConcurrentDictionary<ConverterTypes, Delegate> _cache;

        static Converter()
        {
            _cache = new ConcurrentDictionary<ConverterTypes, Delegate>();
            
            // Pre-Register for our cache!
            var tryConvertMethods = typeof(Converter).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                     .Where(m => m.Name == "TryConvert");
            foreach (var method in tryConvertMethods)
            {
                if (method.IsGenericMethod)
                {
                    var genericTypes = method.GetGenericArguments();
                    Debug.Assert(genericTypes.Length == 2);

                    var delegateType = typeof(TryConvertDelegate<,>)
                        .MakeGenericType(genericTypes);
                    var result = MethodAdapter.TryAdapt(delegateType, method);
                    if (!result)
                    {
                        Debugger.Break();
                        Hold.Debug(delegateType, result);
                    }
                    _cache[(genericTypes[0], genericTypes[1])] = result.Value!;
                }
                else
                {
                    Debugger.Break();
                }
                
                
            }
        }

        private static Delegate CreateTryConvertDelegate(ConverterTypes types)
        {
            var delegateType = typeof(TryConvertDelegate<,>).MakeGenericType(types.InputType, types.OutputType);
            var sig = MethodSignature.Of(delegateType);
            var dm = DelegateBuilder.CreateDynamicMethod(Dumper.Format($"({types.InputType}){types.OutputType}"), sig);
            IFluentILEmitter emitter = new FluentILEmitter(dm);
            var result = MethodAdapter.TryEmitCast(emitter, types.InputType, types.OutputType);
            if (!result)
            {
                Debugger.Break();
                throw new NotImplementedException();
            }
            emitter.Return();
            return dm.CreateDelegate(delegateType);
        }

        public static Result TryConvert<TIn, TOut>([AllowNull] TIn input, 
                                                   [MaybeNullWhen(false)] out TOut output,
                                                   ConvertOptions options = default)
        {
            var delegat = _cache.GetOrAdd((typeof(TIn), typeof(TOut)), CreateTryConvertDelegate);
            Debug.Assert(delegat != null);
            if (delegat is TryConvertDelegate<TIn, TOut> tryConvert)
            {
                return tryConvert(input, out output, options);
            }

            output = default;
            Debugger.Break();
            return new NotImplementedException();
        }

    }


}