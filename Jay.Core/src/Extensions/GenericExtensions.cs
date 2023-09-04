#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using static InlineIL.IL;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Jay.Extensions;

public static class GenericExtensions
{
    /// <summary>
    /// Pushes this <paramref name="value" /> to an <see langword="out" /> <paramref name="output" />.
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
    /// Starts an <see cref="IEnumerable{T}" /> yielding the given <paramref name="value" />.
    /// </summary>
    public static IEnumerable<T?> StartEnumerable<T>(this T? value)
    {
        yield return value;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public static T? IfNull<T>(this T? value, T? fallback)
        where T : class
    {
        if (value is null)
            return fallback; // Okay because of return annotation
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny<T>(this T? value, T? first)
    {
        return EqualityComparer<T>.Default.Equals(value, first);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny<T>(
        this T? value, T? first,
        T? second)
    {
        return EqualityComparer<T>.Default.Equals(value, first) ||
            EqualityComparer<T>.Default.Equals(value, second);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny<T>(
        this T? value, T? first,
        T? second, T? third)
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
        }
        return false;
    }

    public static bool EqualsAny<T>(this T? value, IEnumerable<T?> options)
    {
        foreach (T? option in options)
        {
            if (EqualityComparer<T>.Default.Equals(value, option))
        }
        return false;
    }

    public static string ToNonNullString<T>(this T? value, string? fallback = "")
    {
        if (value is null)
            return (fallback ?? "");
        string? str = value.ToString();
        if (str is null)
            return (fallback ?? "");
        return str;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <paramref name="value"/>
    /// </typeparam>
    /// <param name="value">
    /// The value to check for <c>null</c>
    /// </param>
    /// <param name="exceptionMessage">
    /// An optional message to include if a <see cref="ArgumentNullException"/> is thrown
    /// </param>
    /// <param name="valueName">
    /// The name of the <paramref name="value"/> argument, passed to <see cref="ArgumentNullException"/>
    /// </param>
    /// <returns>
    /// Returns a non-<c>null</c> <paramref name="value"/>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is <c>null</c>
    /// </exception>
    [return: NotNull]
    public static T ThrowIfNull<T>(
        [AllowNull] [NotNull] this T? value,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is not null)
            return value;
        throw new ArgumentNullException(
            valueName,
            exceptionMessage ?? $"The given {typeof(T).Name} value must not be null");
    }
}