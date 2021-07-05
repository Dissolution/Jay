using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Jay.Text;

namespace Jay.Conversion.New
{
    public static partial class Converter
    {
        static Converter()
        {
            
        }

        public static Result TryConvert<TValue, TResult>([AllowNull] TValue value, ConvertOptions options, [MaybeNull] out TResult result)
        {
            // Fast string stuff
            if (value is string text)
                return TryConvert(text, options, out result);
            var valueType = typeof(TValue);
            try
            {
                object? objResult = Convert.ChangeType(value, typeof(TResult), options.FormatProvider);
                if (objResult.Is(out result))
                {
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                result = default;
                return ex;
            }
            
            // TODO: A whole mess of implicit/explicit checking
            result = default;
            return ConversionException.Create<TValue, TResult>();
        }

        public static Result TryConvert<TValue>([AllowNull] TValue value, ConvertOptions options, out string? text)
        {
            if (options.HasExactFormat(out var exactFormat))
            {
                if (SpanFormattable<TValue>.CanFormat)
                {
                    using (ArrayPool<char>.Shared.Rent(TextBuilder.DEFAULT_CAPACITY, out char[] array))
                    {
                        if (SpanFormattable<TValue>.TryFormat(value, array, out int charsWritten,
                                                              exactFormat, options.FormatProvider))
                        {
                            text = new string(array, 0, charsWritten);
                            return true;
                        }
                    }
                }

                if (value is IFormattable formattable)
                {
                    try
                    {
                        text = formattable.ToString(exactFormat, options.FormatProvider);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        text = null;
                        return ex;
                    }
                }
            }

            // Always fallback to ToString()
            text = value?.ToString();
            return true;
        }
    }
}



/*using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Emission;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Jay.Conversion
{
    internal static class Converter
    {
        private static readonly ConcurrentDictionary<(Type? inType, Type? outType), Delegate?> _cache;

        static Converter()
        {
            _cache = new ConcurrentDictionary<(Type? inType, Type? outType), Delegate?>();
            
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions
            
            // sbyte
            Type inType = typeof(sbyte);
            foreach (var outType in new Type[] {typeof(short), typeof(int), typeof(long), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((sbyte value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });

            // byte
            inType = typeof(byte);
            foreach (var outType in new Type[] {typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((byte value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });

            // short
            inType = typeof(short);
            foreach (var outType in new Type[] {typeof(int), typeof(long), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((short value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });

            // ushort
            inType = typeof(ushort);
            foreach (var outType in new Type[] {typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((ushort value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });
            
            // int
            inType = typeof(int);
            foreach (var outType in new Type[] {typeof(long), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((int value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });

            // uint
            inType = typeof(uint);
            foreach (var outType in new Type[] {typeof(long), typeof(ulong), typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((uint value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });
            
            // long
            inType = typeof(long);
            foreach (var outType in new Type[] {typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((long value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });

            // ulong
            inType = typeof(ulong);
            foreach (var outType in new Type[] {typeof(float), typeof(double)})
            {
                _cache[(inType, outType)] = GetUnmanagedConvert(inType, outType);
            }
            SetTryConvert((ulong value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });
            
            // float
            inType = typeof(float);
            _cache[(inType, typeof(double))] = GetUnmanagedConvert(inType, typeof(double));
            SetTryConvert((float value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });
            
            // double
            SetTryConvert((double value, out decimal converted) =>
            {
                converted = new decimal(value);
                return true;
            });
        }

        private static readonly FieldInfo _resultPassField =
            typeof(Result).GetField(nameof(Result._defaultPassResult), Reflect.StaticFlags)
            ?? throw new InvalidOperationException();

        private static readonly ConstructorInfo _resultFailCtor =
            typeof(Result).GetConstructor(Reflect.InstanceFlags,
                                          typeof(bool),
                                          typeof(Exception)) ??
            throw new InvalidOperationException();
        
        private static readonly ConstructorInfo _conversionExceptionCtor =
            typeof(ConversionException).GetConstructor(Reflect.InstanceFlags,
                                                       typeof(Type),
                                                       typeof(Type)) ??
            throw new InvalidOperationException();

        private static ILEmitter LoadPassResult(this ILEmitter emitter)
        {
            return emitter.Ldfld(_resultPassField);
        }

        private static ILEmitter LoadFailResult(this ILEmitter emitter, Type inType, Type outType)
        {
            return emitter.Ldc_I4(0)
                          .Ldtoken(inType)
                          .Ldtoken(outType)
                          .Newobj(_conversionExceptionCtor)
                          .Newobj(_resultFailCtor);
        }
   
        private static Type GetTryConvertDelegateType(Type inType, Type outType) => typeof(TryConvert<,>)
            .MakeGenericType(inType, outType);
        
        
        private static Delegate? GetUnmanagedConvert(Type inType, Type outType)
        {
            return DelegateBuilder.EmitDelegate(GetTryConvertDelegateType(inType, outType),
                                                emitter => emitter.Ldarg(0)
                                                                  .Conv(outType)
                                                                  .Starg(1)
                                                                  .LoadPassResult()
                                                                  .Ret());
        }
        

        private static Delegate? GetTryConvert((Type? inType, Type? outType) types)
        {
            throw new NotImplementedException();    
        }

        private static void SetTryConvert<TIn, TOut>(TryConvert<TIn, TOut>? tryConvert)
        {
            _cache[(typeof(TIn), typeof(TOut))] = tryConvert;
        }


        public static Result TryConvert<TIn, TOut>([AllowNull] TIn value, [MaybeNull] out TOut converted)
        {
            Delegate? del = _cache.GetOrAdd((typeof(TIn), typeof(TOut)), GetTryConvert);
            if (del is null)
            {
                converted = default(TOut);
                return ConversionException.Create<TIn, TOut>();
            }

            if (!(del is TryConvert<TIn, TOut> tryConvert))
            {
                converted = default(TOut);
                return new InvalidOperationException("GetTryConvertFailed");
            }

            return tryConvert(value, out converted);
        }

    }
}*/