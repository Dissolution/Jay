using static InlineIL.IL;

namespace Jay;

public static class ObjectExtensions
{
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
    public static bool Is<T>(this object? input, [NotNullWhen(true)] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }
        else
        {
            output = default;
            return false;
        }
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