using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using InlineIL;
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
           // if (value is TOut output)
           // {
           //     typed = output;
           //     return true;
           // }
           // else
           // {
           //     typed = default(TOut);
           //     return false;
           // }
           
           /*
              .locals init (
      [0] !!1/*TOut* / output
    )

    // [86 12 - 86 37]
    IL_0000: ldarg.0      // 'value'
    IL_0001: box          !!0/*TIn* /
    IL_0006: isinst       !!1/*TOut* /
    IL_000b: brfalse.s    IL_0027
    IL_000d: ldarg.0      // 'value'
    IL_000e: box          !!0/*TIn* /
    IL_0013: isinst       !!1/*TOut* /
    IL_0018: unbox.any    !!1/*TOut* /
    IL_001d: stloc.0      // output

    // [88 16 - 88 31]
    IL_001e: ldarg.1      // typed
    IL_001f: ldloc.0      // output
    IL_0020: stobj        !!1/*TOut* /

    // [89 16 - 89 28]
    IL_0025: ldc.i4.1
    IL_0026: ret

    // [93 16 - 93 38]
    IL_0027: ldarg.1      // typed
    IL_0028: initobj      !!1/*TOut* /

    // [94 16 - 94 29]
    IL_002e: ldc.i4.0
    IL_002f: ret
            
            */
           
           DeclareLocals(new LocalVar("output", typeof(TOut)));
           
           Emit.Ldarg(nameof(value));
           Emit.Box<TIn>();
           Emit.Isinst<TOut>();
           Emit.Brfalse("isnot");
           
           Emit.Ldarg(nameof(value));
           Emit.Box<TIn>();
           Emit.Isinst<TOut>();
           Emit.Unbox_Any<TOut>();
           Emit.Stloc("output");
           Emit.Ldarg(nameof(typed));
           Emit.Ldloc("output");
           Emit.Stobj<TOut>();
           Emit.Ldc_I4_1();
           Emit.Ret();
           MarkLabel("isnot");
           Emit.Ldarg(nameof(typed));
           Emit.Initobj<TOut>();
           Emit.Ldc_I4_0();
           Emit.Ret();
           throw Unreachable();
        }

        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOut As<TIn, TOut>(this TIn value)
        {
            if (value is TOut output)
                return output;
            throw new ArgumentException("Input value cannot be cast to output type", nameof(value));
        }
    }
}