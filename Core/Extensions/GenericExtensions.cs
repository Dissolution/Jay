﻿using static InlineIL.IL;

namespace Jay.Extensions;

public static class GenericExtensions
{
    /// <summary>
    /// Pushes this <paramref name="value"/> to an <see langword="out"/> <paramref name="output"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Out<T>(this T value, out T output)
    {
        output = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<T>(this T? value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Brfalse("isDefault");
        Emit.Ldc_I4_0();
        Emit.Ret();
        MarkLabel("isDefault");
        Emit.Ldc_I4_1();
        Emit.Ret();
        throw Unreachable();
    }

    /// <summary>
    /// Starts an <see cref="IEnumerable{T}"/> yielding the given <paramref name="value"/>.
    /// </summary>
    public static IEnumerable<T?> StartEnumerable<T>(this T? value)
    {
        yield return value;
    }

    [return: NotNullIfNotNull("fallback")]
    public static T? IfNull<T>(this T? value, T? fallback)
        where T : class
    {
        if (value is null)
            return fallback;  // Okay because of return annotation
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

  
}