using Jay.Reflection;

namespace Jay.Validation;

public static class ValidationExtensions
{
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
        [AllowNull, NotNull] this T? value,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is not null)
            return value;

        throw new ArgumentNullException(
            valueName,
            exceptionMessage ?? $"The given {typeof(T).ToCode()} value must not be null");
    }

    /// <summary>
    /// Casts this <see cref="object"/> <c>as</c> a <typeparamref name="TOut"/> value and returns it
    /// </summary>
    /// <param name="obj">
    /// The <see cref="object"/> to convert to a <typeparamref name="TOut"/> value
    /// </param>
    /// <param name="exceptionMessage">
    /// An optional message for the <see cref="ArgumentException"/> that is thrown if <paramref name="obj"/> is not a valid <typeparamref name="TOut"/> value
    /// </param>
    /// <param name="objName">
    /// The captured name for the <paramref name="obj"/> parameter, used with an <see cref="ArgumentException"/>
    /// </param>
    /// <typeparam name="TOut">
    /// The <see cref="Type"/> of value to cast <paramref name="obj"/> <c>as</c>
    /// </typeparam>
    /// <returns>
    /// <c>obj as TOut</c>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown is <paramref name="obj"/> is not a valid <typeparamref name="TOut"/> value
    /// </exception>
    [return: NotNull]
    public static TOut AsValid<TOut>(
        [AllowNull, NotNull]
        this object? obj,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        if (obj is TOut output)
            return output;

        throw new ArgumentException(
            exceptionMessage ?? $"The given {obj?.GetType().ToCode()} value is not a valid {typeof(TOut).ToCode()} instance",
            objName);
    }
}