using Jay.Collections;
using Jay.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Debugging;

namespace Jay.Text
{
    internal delegate bool TryFormatSig<in T>([AllowNull] T value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
    internal delegate string FormatSig<in T>([AllowNull] T value, string? format, IFormatProvider? provider);
    
    public static class Formatter
    {
        private static readonly ConcurrentTypeCache<TryFormatSig<object?>> _tryFormatCache;
        private static readonly ConcurrentTypeCache<FormatSig<object?>> _formatCache;
        
        internal static MethodInfo TryFormatMethod;
        internal static MethodInfo FormatMethod;
        
        static Formatter()
        {
            _tryFormatCache = new ConcurrentTypeCache<TryFormatSig<object?>>();
            _formatCache = new ConcurrentTypeCache<FormatSig<object?>>();
            TryFormatMethod = typeof(Formatter).GetMethod(nameof(TryFormat),
                                                          BindingFlags.Public | BindingFlags.Static)
                                               .ThrowIfNull();
            FormatMethod = typeof(Formatter).GetMethod(nameof(Format),
                                                          BindingFlags.Public | BindingFlags.Static)
                                               .ThrowIfNull();
        }

        internal static Type[] GetTryFormatSigArgTypes(Type valueType) => new Type[5]
            {
                valueType, 
                typeof(Span<char>), 
                typeof(int).MakeByRefType(), 
                typeof(ReadOnlySpan<char>),
                typeof(IFormatProvider)
            };
        
        internal static Type[] GetFormatSigArgTypes(Type valueType) => new Type[3]
        {
            valueType, 
            typeof(string),
            typeof(IFormatProvider)
        };

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

            var tryFormat = _tryFormatCache.GetOrAdd(value.GetType(), CreateTryFormatDelegate);
            return tryFormat(value, destination, out charsWritten, format, provider);
        }

        public static string Format(object? value,
                                    string? format = default,
                                    IFormatProvider? provider = default)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var formatter = _formatCache.GetOrAdd(value.GetType(), CreateFormatDelegate);
            return formatter(value, format, provider);
        }

        private static TryFormatSig<object?> CreateTryFormatDelegate(Type type)
        {
            // Be sure that Formatter<T> has been constructed
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            // Get the method from that class
            var tryFormatMethod = type.GetMethod("TryFormat",
                           BindingFlags.Public | BindingFlags.Static,
                           null,
                           GetTryFormatSigArgTypes(type),
                           null);
            tryFormatMethod.ThrowIfNull(nameof(tryFormatMethod), $"Cannot find Formatter<{type.Name}>.TryFormat");
            return DelegateBuilder.Build<TryFormatSig<object?>>(emitter => emitter.Ldarg(0)
                                                                    .UnboxOrCastclass(type)
                                                                    .Ldarg(1)
                                                                    .Ldarg(2)
                                                                    .Ldarg(3)
                                                                    .Ldarg(4)
                                                                    .Call(tryFormatMethod!)
                                                                    .Ret());
        }
        
        private static FormatSig<object?> CreateFormatDelegate(Type type)
        {
            // Be sure that Formatter<T> has been constructed
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            // Get the method from that class
            var formatMethod = type.GetMethod("Format",
                                              BindingFlags.Public | BindingFlags.Static,
                                              null,
                                              GetFormatSigArgTypes(type),
                                              null);
            formatMethod.ThrowIfNull(nameof(formatMethod), $"Cannot find Formatter<{type.Name}>.Format");
            return DelegateBuilder.Build<FormatSig<object?>>(emitter => emitter.Ldarg(0)
                                                                    .UnboxOrCastclass(type)
                                                                    .Ldarg(1)
                                                                    .Ldarg(2)
                                                                    .Call(formatMethod!)
                                                                    .Ret());
        }
    }
    
    public static class Formatter<T>
    {
        private static readonly TryFormatSig<T> _tryFormat;
        private static readonly FormatSig<T> _format;
        
        static Formatter()
        {
            var type = typeof(T);
            if (type == typeof(object))
            {
                _tryFormat = MethodAdapter.Adapt<TryFormatSig<T>>(Formatter.TryFormatMethod);
                _format = MethodAdapter.Adapt<FormatSig<T>>(Formatter.FormatMethod);
                return;
            }

            // Try to find TryFormat first
            var tryFormatMethod = type.GetMethod("TryFormat",
                                                 Reflect.InstanceFlags,
                                                 null,
                                                 Formatter.GetTryFormatSigArgTypes(type),
                                                 null);
            if (tryFormatMethod is null)
            {
                _tryFormat = FailTryFormat;
            }
            else
            {
                _tryFormat = MethodAdapter.Adapt<TryFormatSig<T>>(tryFormatMethod);
            }
                
            // Look for Format
            
            // string ToString(string? format, IFormatProvider? formatProvider);
            MethodInfo? formatMethod = type.GetMethod("ToString",
                                                      Reflect.InstanceFlags,
                                                      null,
                                                      new Type[] {typeof(string), typeof(IFormatProvider)},
                                                      null);
            if (formatMethod != null)
            {
                _format = MethodAdapter.Adapt<FormatSig<T>>(formatMethod);
                return;
            }

            // string ToString(string? format);
            formatMethod = type.GetMethod("ToString",
                                          Reflect.InstanceFlags,
                                          null,
                                          new Type[] {typeof(string)},
                                          null);
            if (formatMethod != null)
            {
                _format = MethodAdapter.Adapt<FormatSig<T>>(formatMethod);
                return;
            }

            _format = DefaultFormat;
        }
        
        private static bool FailTryFormat(T? value,
                                          Span<char> destination,
                                          out int charsWritten,
                                          ReadOnlySpan<char> format,
                                          IFormatProvider? provider)
        {
            charsWritten = 0;
            return false;
        }

        private static string DefaultFormat(T? value,
                                            string? format,
                                            IFormatProvider? provider)
        {
            return value?.ToString() ?? string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormat(T? value,
                                     Span<char> destination,
                                     out int charsWritten,
                                     ReadOnlySpan<char> format = default,
                                     IFormatProvider? provider = null)
        {
            return _tryFormat(value, destination, out charsWritten, format, provider);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(T? value,
                                     string? format = default,
                                     IFormatProvider? provider = null)
        {
            return _format(value, format, provider);
        }
    }
}