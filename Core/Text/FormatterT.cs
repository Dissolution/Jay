using Jay.Collections;
using Jay.Reflection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Reflection.Comparison;
using Jay.Reflection.Emission;

namespace Jay.Text
{
    internal delegate bool TryFormatObject(object? obj, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
    internal delegate string FormatObject(object? obj, string? format, IFormatProvider? provider);
    
    internal readonly struct FormatMethods : IEquatable<FormatMethods>
    {
        public readonly Type ObjectType;
        public readonly TryFormatObject TryFormatObject;
        public readonly FormatObject FormatObject;

        public FormatMethods(Type objectType, TryFormatObject tryFormatObject, FormatObject formatObject)
        {
            this.ObjectType = objectType;
            this.TryFormatObject = tryFormatObject;
            this.FormatObject = formatObject;
        }

        public bool Equals(FormatMethods formatMethods)
        {
            return this.ObjectType == formatMethods.ObjectType;
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is FormatMethods formatMethods && Equals(formatMethods);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(ObjectType);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Dumper.Format($"FormatMethods<{ObjectType}>");
        }
    }
    
   
    
    public static class Formatter
    {
        private static readonly ConcurrentTypeCache<FormatMethods> _formatMethods;
      
        static Formatter()
        {
            _formatMethods = new ConcurrentTypeCache<FormatMethods>
            {
                [typeof(string)] = new FormatMethods(typeof(string), TryFormatString, FormatString),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool FailTryFormatObject(object? obj, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            charsWritten = 0;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatString(object? obj, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            ReadOnlySpan<char> text = (string?) obj;
            if (text.TryCopyTo(destination))
            {
                charsWritten = text.Length;
                return true;
            }

            charsWritten = 0;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string DefaultFormatObject(object? obj, string? format, IFormatProvider? provider)
        {
            // We know that obj is not IFormattable
            return obj?.ToString() ?? string.Empty;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatString(object? obj, string? format, IFormatProvider? provider)
        {
            return (obj as string) ?? string.Empty;
        }

        private static FormatMethods GetFormatMethods(Type type)
        {
            return _formatMethods.GetOrAdd(type, CreateFormatMethods);
        }
        
        private static FormatMethods CreateFormatMethods(Type type)
        {
            // Check for TryFormat
            var tryFormatMethods = type.GetMethods(Reflect.InstanceFlags)
                                       .Where(method => method.Name == nameof(byte.TryFormat))
                                       .Where(method => method.ReturnType == typeof(bool))
                                       .OrderByDescending(MethodComplexityComparer.Default)
                                       .ToList();
            TryFormatObject? tryFormatObject = null;
            if (tryFormatMethods.Count == 0)
            {
                tryFormatObject = FailTryFormatObject;
            }
            else if (tryFormatMethods.Count == 1)
            {
                var result = MethodAdapter.TryAdapt<TryFormatObject>(tryFormatMethods[0], Safety.IgnoreExtraParams);
                if (result)
                {
                    tryFormatObject = result._value!;
                }
                else
                {
                    tryFormatObject = FailTryFormatObject;
                }
            }
            else
            {
                Debugger.Break();
                
                foreach (var tryFormatMethod in tryFormatMethods)
                {
                    Hold.Debug(tryFormatMethod);
                }

                tryFormatObject = FailTryFormatObject;
            }

            FormatObject? formatObject = null;
            var toStringMethods = type.GetMethods(Reflect.InstanceFlags)
                                    .Where(method => method.Name == nameof(IFormattable.ToString))
                                    .OrderByDescending(MethodComplexityComparer.Default)
                                    .ToList();
            foreach (var toStringMethod in toStringMethods)
            {
                var result = MethodAdapter.TryAdapt<FormatObject>(toStringMethods[0], Safety.IgnoreExtraParams);
                if (result)
                {
                    formatObject = result._value;
                    break;
                }
            }

            if (formatObject is null)
            {
                formatObject = DefaultFormatObject;
            }

            return new FormatMethods(type, tryFormatObject, formatObject);
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
            return GetFormatMethods(value.GetType())
                .TryFormatObject(value, destination, out charsWritten, format, provider);
        }

        public static string Format(object? value,
                                    string? format = default,
                                    IFormatProvider? provider = default)
        {
            if (value is null)
                return string.Empty;
            return GetFormatMethods(value.GetType())
                .FormatObject(value, format, provider);
        }

    }
}