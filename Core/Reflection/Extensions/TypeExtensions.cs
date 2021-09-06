using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Debugging;
using Jay.Reflection.Emission;

namespace Jay.Reflection
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Is this <paramref name="type"/> a signed data <see cref="Type"/>?
        /// </summary>
        public static bool IsSigned(this Type? type)
        {
            if (type is null)
                return false;
            
            if (type.HasElementType)
                return IsSigned(type.GetElementType()!);
            
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.String:
                    return false;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return true;
                default:
                    return false;
            }
        }

        private static readonly ConcurrentTypeCache<bool> _isReferenceCache = new ConcurrentTypeCache<bool>();
        
        public static bool IsReferenceOrContainsReferences(this Type type) => _isReferenceCache.GetOrAdd(type, IsReference);

        private static bool IsReference(Type type)
        {
            var method = typeof(RuntimeHelpers)
                .GetMethod(nameof(RuntimeHelpers.IsReferenceOrContainsReferences), Reflect.StaticFlags)
                .ThrowIfNull(exceptionMessage: $"Unable to find {nameof(RuntimeHelpers.IsReferenceOrContainsReferences)}")
                .MakeGenericMethod(type);
            var result = MethodAdapter.TryAdapt<Func<bool>>(method);
            if (!result.TryGetValue(out var func))
            {
                Debugger.Break();
                throw result.GetException();
            }
            return func!();
        }

        public static ConstructorInfo? GetConstructor(this Type type, BindingFlags bindingAttr, params Type[] types)
        {
            return type.GetConstructor(bindingAttr, null, types, null);
        }

        public static bool IsArray(this Type type, [NotNullWhen(true)] out Type? elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType()!;
                return true;
            }
            else
            {
                elementType = null;
                return false;
            }
        }
    }
}