using Jay.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Jay
{
    public static class TypeExtensions
    {
        public static bool Implements(this Type? type, Type? checkType)
        {
            if (type is null)
                return checkType is null;
            if (checkType is null) return false;
            if (type == checkType) return true;
            if (type.IsAssignableTo(checkType))
                return true;
            if (type.IsConstructedGenericType &&
                type.GetGenericTypeDefinition() == checkType)
                return true;
            // TODO: A LOT MORE
            return false;
        }

        public static bool Implements<TCheck>(this Type? type) => Implements(type, typeof(TCheck));
        
        /// <summary>
        /// Is this <see cref="Type"/> a <see cref="Nullable{T}"/> type?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type? type)
        {
            if (type is null) return false;
            return type.IsConstructedGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullable(this Type? type, [NotNullWhen(true)] out Type? underlyingType)
        {
            if (type != null && type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                underlyingType = type.GetGenericArguments()[0];
                return true;
            }
            underlyingType = null;
            return false;
        }
        
        

        /// <summary>
        /// Can objects of this <see cref="Type"/> be assigned Null?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanBeNull(this Type? type)
        {
            return type is null || !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() != typeof(Nullable<>));
        }

        public static MethodInfo? GetInvokeMethod(this Type? type) 
            => type?.GetMethod("Invoke", Reflect.InstanceFlags);
    }
}