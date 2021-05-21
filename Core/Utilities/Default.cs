using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using static InlineIL.IL;

namespace Jay
{
    public static class Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? For<T>() => default(T);
        
        public static object? For(Type? type)
        {
            if (type is null)
                return null;
            if (type.IsClass)
                return null;
            return Activator.CreateInstance(type);
        }
        
        /// <summary>
        /// Create a new, Uninitialized object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [return: NotNull]
        public static T Raw<T>()
        {
            return (T)RuntimeHelpers.GetUninitializedObject(typeof(T))!;
        }

        /// <summary>
        /// Create a new, Uninitialized object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull("type")]
        public static object? Raw(Type? type)
        {
            if (type is null) return null;
            return RuntimeHelpers.GetUninitializedObject(type);
        }
    }
}