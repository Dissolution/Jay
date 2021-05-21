using Jay.Debugging;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Jay.Reflection.Emission
{
    public partial class ILEmitter
    {
        /// <summary>
        /// Gets the correct <see cref="OpCode"/> to call the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static OpCode GetCallOpCode(MethodBase method)
        {
            //If the method is static, we know it can never be null, so we can Call
            if (method.IsStatic)
                return OpCodes.Call;
            //If the method owner is a struct, it can also never be null, so we can Call
            if ((method.ReflectedType != null && method.ReflectedType.IsValueType) ||
                (method.DeclaringType != null && method.DeclaringType.IsValueType))
            {
                return OpCodes.Call;
            }
            return OpCodes.Callvirt;
        }
        
        /// <summary>
        /// Asserts the given <see cref="Type"/> is non-<see langword="null"/> and is the type of a <see langword="struct"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown is <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not a value <see cref="Type"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsValueType(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsValueType)
                throw new ArgumentException($"{type} is not a value type", nameof(type));
        }

        /// <summary>
        /// Asserts that the given generic type <typeparamref name="T"/> is a value <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to validate.</typeparam>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not a value <see cref="Type"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsValueType<T>()
        {
            if (!typeof(T).IsValueType)
                throw new ArgumentException($"{typeof(T)} is not a value type", nameof(T));
        }
        
        /// <summary>
        /// Asserts the given <see cref="Type"/> is non-<see langword="null"/> and is the type of a <see langword="class"/> or <see langdwor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown is <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not a class <see cref="Type"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsClassType(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsValueType)
                throw new ArgumentException($"{type} is not a class type", nameof(type));
        }

        /// <summary>
        /// Asserts that the given generic type <typeparamref name="T"/> is a class <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to validate.</typeparam>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not a class <see cref="Type"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsClassType<T>()
        {
            if (typeof(T).IsValueType)
                throw new ArgumentException($"{typeof(T)} is not a class type", nameof(T));
        }
    }
}