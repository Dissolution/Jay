﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using static InlineIL.IL;

namespace Jay;

public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull(this object? obj)
    {
        Emit.Ldarg(nameof(obj));
        Emit.Ldnull();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault(this object? obj)
    {
        Emit.Ldarg(nameof(obj));
        Emit.Brfalse("yup");
        Emit.Ldc_I4_0();
        Emit.Ret();
        MarkLabel("yup");
        Emit.Ldc_I4_1();
        Emit.Ret();
        throw Unreachable();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<TOut>(this object? input, [NotNullWhen(true)] out TOut output)
    {
        // if (input is TOut temp)
        // {
        //     output = temp;
        //     return true;
        // }
        // else
        // {
        //     output = default;
        //     return false;
        // }

        Emit.Ldarg(nameof(input));
        Emit.Isinst<TOut>();
        Emit.Brfalse("isnot");

        Emit.Ldarg(nameof(output));
        Emit.Ldarg(nameof(input));
        Emit.Unbox_Any<TOut>();
        Emit.Stobj<TOut>();
        Emit.Ldc_I4_1();
        Emit.Ret();

        MarkLabel("isnot");
        Emit.Ldarg(nameof(output));
        Emit.Initobj<TOut>();
        Emit.Ldc_I4_0();
        Emit.Ret();

        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanBe<T>(this object? obj, out T? value)
    {
        if (obj is null)
        {
            value = default;
            return !typeof(T).IsValueType || typeof(T).IsAssignableTo(typeof(Nullable<>));
        }
        else
        {
            return obj.Is<T>(out value);
        }
    }
}