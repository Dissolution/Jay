using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Jay.Comparison;
using Jay.Text;

namespace Jay.Conversion.New
{
    public static class StaticTypeCache
    {
        private static readonly ConcurrentDictionary<Type[], Type> _cache;

        static StaticTypeCache()
        {
            _cache = new ConcurrentDictionary<Type[], Type>(new ArrayEqualityComparer<Type>());
        }
    }
    
    public static class Converter<TIn, TOut>
    {
        private static readonly Type _inType = typeof(TIn);
        private static readonly Type _outType = typeof(TOut);
        private static readonly TryConvert<TIn, TOut> _tryConvert;
        
        static Converter()
        {
            bool isInSpanFormattable = SpanFormattable<TIn>.CanFormat;

            string methodName;
            if (_outType == typeof(string))
            {
                if (isInSpanFormattable)
                {
                    methodName = nameof(TryConvertSpanFormattableToString);
                }
                else
                {
                    methodName = nameof(TryConvertToString);
                }
            }
            else
            {
                methodName = nameof(TryConvert);
            }

            var myMethod = typeof(Converter<TIn, TOut>).GetMethod(methodName, 
                                                                  BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (myMethod is null)
            {
                throw new InvalidOperationException();
            }
            _tryConvert = (Delegate.CreateDelegate(typeof(TryConvert<TIn, TOut>), myMethod)
                               as TryConvert<TIn, TOut>)
                .ThrowIfNull("_tryConvert == null");

        }

        private static Result TryConvertToString([AllowNull] TIn value, ConvertOptions options, out string? text)
        {
            if (options.HasExactFormat(out var exactFormat))
            {
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
                        return ConversionException.Create<TIn, string>(value, ex);
                    }
                }
            }

            // Always fallback to ToString()
            text = value?.ToString();
            return true;
        }
        
        private static Result TryConvertSpanFormattableToString([AllowNull] TIn value, ConvertOptions options, out string? text)
        {
            if (options.HasExactFormat(out var exactFormat))
            {
                using (ArrayPool<char>.Shared.Rent(TextBuilder.DEFAULT_CAPACITY, out char[] array))
                {
                    if (SpanFormattable<TIn>.TryFormat(value, array, out int charsWritten, 
                                                       exactFormat, options.FormatProvider))
                    {
                        text = new string(array, 0, charsWritten);
                        return true;
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
                        return ConversionException.Create<TIn, string>(value, ex);
                    }
                }
            }

            // Always fallback to ToString()
            text = value?.ToString();
            return true;
        }
        
        public static Result TryConvert([AllowNull] TIn value, ConvertOptions options, [MaybeNull] out TOut result)
        {
            // Fast string stuff
            try
            {
                object? objResult = Convert.ChangeType(value, _outType, options.FormatProvider);
                if (objResult.Is(out result))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                result = default;
                return ConversionException.Create<TIn, TOut>(value, ex);
            }
            
            // TODO: A whole mess of implicit/explicit checking
            result = default;
            return ConversionException.Create<TIn, TOut>(value);
        }
    }
}