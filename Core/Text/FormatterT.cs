using Jay.Collections;
using Jay.Reflection;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jay.Text
{
    internal delegate bool TryFormatSig<in T>(T value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
    internal delegate string? FormatSig<in T>(T value, ReadOnlySpan<char> format, IFormatProvider? provider);

    
    public static class Formatter
    {
        private static readonly ConcurrentTypeCache<Delegate> _tryFormatCache;

        static Formatter()
        {
            _tryFormatCache = new ConcurrentTypeCache<Delegate>();
        }

        public static bool TryFormat(object? value,
                                     Span<char> destination,
                                     out int charsWritten,
                                     ReadOnlySpan<char> format = default,
                                     IFormatProvider? provider = null)
        {
            if (value is null)
            {
                charsWritten = 0;
                return true;
            }
            else
            {
                var type = value.GetType();
                var del = _tryFormatCache.GetOrAdd(type, CreateTryFormatDelegate);
                if (del is TryFormatSig<object?> tryFormat)
                {
                    return tryFormat(value, destination, out charsWritten, format, provider);
                }
                else
                {
                    charsWritten = 0;
                    return false;
                }
            }
        }

        private static Delegate CreateTryFormatDelegate(Type type)
        {
            throw new NotImplementedException();
        }
    }
    
    public static class Formatter<T>
    {
        private static readonly Type[] TryFormatSigArgTypes = new Type[5]
        {
            typeof(T), 
            typeof(Span<char>), 
            typeof(int).MakeByRefType(), 
            typeof(ReadOnlySpan<char>),
            typeof(IFormatProvider)
        };
        
        
        private static readonly TryFormatSig<T> _tryFormat;
        private static readonly FormatSig<T> _format;
        
        static Formatter()
        {
            /*
            var type = typeof(T);
            if (type == typeof(object))
            {
                _tryFormat = FailTryFormat;
                _format = DefaultFormat;
            }
            else
            {
                var tryFormatMethod = type.GetMethod("TryFormat",
                                                     Reflect.InstanceFlags,
                                                     null,
                                                     TryFormatSigArgTypes,
                                                     null);
                if (tryFormatMethod is null)
                {
                    _tryFormat = FailTryFormat;
                }
                else
                {
                    _tryFormat = tryFormatMethod.CreateDelegate<TryFormatSig<T>>();
                }
                
                // string ToString(string? format, IFormatProvider? formatProvider);
                MethodInfo? formatMethod = type.GetMethod("ToString",
                                                          Reflect.InstanceFlags,
                                                          null,
                                                          new Type[] {typeof(string), typeof(IFormatProvider)},
                                                          null);
                if (formatMethod != null)
                {
                    _format = formatMethod.CreateDelegate<FormatSig<T>>();
                }
                else
                {
                    formatMethod = type.GetMethod("ToString",
                                                  Reflect.InstanceFlags,
                                                  null,
                                                  new Type[] {typeof(string)},
                                                  null);
                    if (formatMethod != null)
                    {
                        _format = formatMethod.CreateDelegate<FormatSig<T>>();
                    }
                    else
                    {
                        _format = DefaultFormat;
                    }
                }
            }
            */
        }
        
        private static bool FailTryFormat(T value,
                                       Span<char> destination,
                                       out int charsWritten,
                                       ReadOnlySpan<char> format,
                                       IFormatProvider? provider)
        {
            charsWritten = 0;
            return false;
        }

        private static string? DefaultFormat(T value,
                                             ReadOnlySpan<char> format,
                                             IFormatProvider? provider)
        {
            return value?.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormat(T value,
                                     Span<char> destination,
                                     out int charsWritten,
                                     ReadOnlySpan<char> format = default,
                                     IFormatProvider? provider = null)
        {
            //return _tryFormat(value, destination, out charsWritten, format, provider);
            charsWritten = 0;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? Format(T value,
                                     ReadOnlySpan<char> format = default,
                                     IFormatProvider? provider = null)
        {
            //return _format(value, format, provider);
            if (value is IFormattable formattable)
            {
                return formattable.ToString(new string(format), provider);
            }
            else
            {
                return value?.ToString();
            }
        }
    }
}