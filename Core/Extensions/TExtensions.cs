using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static InlineIL.IL;

namespace Jay
{
    public static class TExtensions
    {
        /// <summary>
        /// Starts an <see cref="IEnumerable{T}"/> yielding the given <paramref name="value"/>.
        /// </summary>
        public static IEnumerable<T?> StartEnumerable<T>(this T? value)
        {
            yield return value;
        }

        /// <summary>
        /// Is this <paramref name="value"/> equal to <see langword="default{T}"/>?
        /// </summary>
        public static bool IsDefault<T>(this T? value) => EqualityComparer<T>.Default.Equals(value, default(T));

        [return: NotNullIfNotNull("fallback")]
        public static T IfNull<T>([AllowNull] this T value, [AllowNull] T fallback)
            where T : class
        {
            if (value is null)
                return fallback!;  // Okay because of return annotation
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T? value, T? first)
        {
            return EqualityComparer<T>.Default.Equals(value, first);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T? value, T? first, T? second)
        {
            return EqualityComparer<T>.Default.Equals(value, first) ||
                   EqualityComparer<T>.Default.Equals(value, second);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T? value, T? first, T? second, T? third)
        {
            return EqualityComparer<T>.Default.Equals(value, first) ||
                   EqualityComparer<T>.Default.Equals(value, second) ||
                   EqualityComparer<T>.Default.Equals(value, third);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAny<T>(this T? value, params T?[] options)
        {
            for (var i = 0; i < options.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(value, options[i]))
                    return true;
            }
            return false;
        }
        
        public static bool EqualsAny<T>(this T? value, IEnumerable<T?> options)
        {
            foreach (var option in options)
            {
                if (EqualityComparer<T>.Default.Equals(value, option))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Is this <paramref name="value"/> the given <typeparamref name="TOut"/> value and if so, get it converted to that type.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="value"></param>
        /// <param name="typed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is<TIn, TOut>(this TIn value, [MaybeNullWhen(false)] out TOut typed)
        {
            throw new NotImplementedException();
            // Emit.Ldarg(nameof(typed));
            // Emit.Ldarg(nameof(value));
            // Emit.Box(typeof(TIn));
            // Emit.Isinst(typeof(TOut));
            // Emit.Stobj(typeof(TOut));
            // Emit.Ldarg(nameof(typed));
            // Emit.Ldobj(typeof(TOut));
            // Emit.Box(typeof(TOut));
            // Emit.Ldnull();
            // Emit.Cgt_Un();
            // Emit.Ret();
            // throw Unreachable();
            //
            // if (value is TOut)
            // {
            //     typed = (TOut) value;
            //     return true;
            // }
            // else
            // {
            //     typed = default(TOut);
            //     return false;
            // }
            //
            // //if (value is TOut outTyped)
            // //{
            // //    typed = outTyped;
            // //    return true;
            // //}
            // //else
            // //{
            // //    typed = default!;
            // //    return false;
            // //}
        }
    }
}