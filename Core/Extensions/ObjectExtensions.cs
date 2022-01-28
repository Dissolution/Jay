using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Reflection;
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
    public static ref T UnboxRef<T>(this object? obj)
    {
        Emit.Ldarg(nameof(obj));
        Emit.Isinst<T>();
        Emit.Brfalse("isNotT");

        Emit.Ldarg(nameof(obj));
        Emit.Unbox<T>();
        Emit.Ret();

        MarkLabel("isNotT");
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>(this object? input, [NotNullWhen(true)] out T output)
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
        Emit.Isinst<T>();
        Emit.Brfalse("isNotT");

        Emit.Ldarg(nameof(output));
        Emit.Ldarg(nameof(input));
        Emit.Unbox_Any<T>();
        Emit.Stobj<T>();
        Emit.Ldc_I4_1();
        Emit.Ret();

        MarkLabel("isNotT");
        Emit.Ldarg(nameof(output));
        Emit.Initobj<T>();
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
            return !typeof(T).IsValueType || typeof(T).Implements(typeof(Nullable<>));
        }
        else
        {
            return obj.Is<T>(out value);
        }
    }
}