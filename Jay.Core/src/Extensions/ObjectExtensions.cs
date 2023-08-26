using static InlineIL.IL;

namespace Jay.Extensions;

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
        output = default;
        return false;
    }

    /// <summary>
    /// If this <see cref="object"/> can be a <typeparamref name="T"/> value,
    /// cast it to that value and return <c>true</c>.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// This differs from the <c>is</c> keyword in that,
    /// if <paramref name="obj"/> is <c>null</c> and <typeparamref name="T"/> can contain <c>null</c>,
    /// returning <c>null</c> as <paramref name="value"/> is valid.
    /// </remarks>
    public static bool CanBe<T>(this object? obj, out T? value)
    {
        if (obj is T)
        {
            value = (T)obj;
            return true;
        }
        value = default;
        return obj is null && typeof(T).CanContainNull();
    }

    /// <summary>
    /// Is this <see cref="object" /> <c>null</c>, <see cref="None" />, or <see cref="DBNull" />?
    /// </summary>
    public static bool IsNone([NotNullWhen(false)] this object? obj)
    {
        return obj is null or None or DBNull;
    }

    /// <summary>
    /// Returns this <see cref="object"/> as a <typeparamref name="TOut"/> or throw an <see cref="ArgumentException"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="exceptionMessage"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static TOut MustBe<TOut>(
        this object? obj,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(obj))]
        string? valueName = null)
    {
        if (obj is TOut output)
            return output;
        throw new ArgumentException(
            exceptionMessage ?? $"The given {obj?.GetType().Name} value is not a {typeof(TOut).Name} instance",
            valueName);
    }
}