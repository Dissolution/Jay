using Jay.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Jay.Debugging;

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
            return type is null || !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsByRef(this Type? type, out Type underlyingType)
        {
            if (type is null)
            {
                underlyingType = typeof(void);
                return false;
            }

            if (type.IsByRef)
            {
                underlyingType = type.GetElementType()
                                     .ThrowIfNull();
                return true;
            }

            underlyingType = type;
            return false;
        }

        public static (bool ByRef, Type UnderlyingType) IsByRef(this Type? type)
        {
            if (type is null)
            {
                return (false, typeof(void));
            }

            if (type.IsByRef)
            {
                return (true, type.GetElementType()!);
            }

            return (false, type);
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(t => t == interfaceType);
        }
    }
}