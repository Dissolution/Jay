/*using Jay.Debugging;
using Jay.Reflection;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Conversion
{
    public static class Converter
    {
        internal readonly struct Types : IEquatable<Types>
        {
            public readonly Type InputType;
            public readonly Type OutputType;

            public Types(Type inputType, Type outputType)
            {
                this.InputType = inputType;
                this.OutputType = outputType;
            }

            public void Deconstruct(out Type inputType, out Type outputType)
            {
                inputType = this.InputType;
                outputType = this.OutputType;
            }

            public bool Equals(Types types)
            {
                return types.InputType == InputType &&
                       types.OutputType == OutputType;
            }

            public override bool Equals(object? obj)
            {
                if (obj is Types types)
                    return Equals(types);
                return false;
            }

            public override int GetHashCode()
            {
                return Hasher.Create(InputType, OutputType);
            }

            public override string ToString()
            {
                return $"({InputType.Name},{OutputType.Name}";
            }
        }

        private static readonly ConcurrentDictionary<Types, Delegate> _tryConvertCache;

        static Converter()
        {
            _tryConvertCache = new ConcurrentDictionary<Types, Delegate>();
        }
        
        internal static (FieldInfo Field, MethodInfo InvokeMethod) GetTryConvert(Type inputType, Type outputType)
        {
            var converterType = typeof(Converter<,>).MakeGenericType(inputType, outputType);
            RuntimeHelpers.RunClassConstructor(converterType.TypeHandle);
            var field = converterType.GetField("_tryConvertTo",
                                               Reflect.StaticFlags)
                                     .ThrowIfNull();
            var method = field.FieldType.GetMethod("Invoke", Reflect.StaticFlags)
                              .ThrowIfNull();
            return (field, method);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result TryConvertTo<TIn, TOut>([AllowNull] this TIn input, 
                                                     [NotNullIfNotNull("input")] out TOut output)
        {
            return Converter<TIn, TOut>.TryConvertTo(input, out output);
        }
    }

    public delegate Result TryConvertToDelegate<in TIn, TOut>([AllowNull] TIn input, [NotNullIfNotNull("input")] out TOut output);
    
    public static class Converter<TIn, TOut>
    {
        private static TryConvertToDelegate<TIn, TOut> _tryConvertTo;

        static Converter()
        {
            
        }

      
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result TryConvertTo([AllowNull] TIn input, 
                                          [NotNullIfNotNull("input")] out TOut output) =>
            _tryConvertTo(input, out output);
    }
}*/