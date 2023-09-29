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
    /// Returns this <see cref="object"/> as a <typeparamref name="TOut"/> or throw an <see cref="ArgumentException"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="exceptionMessage"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static TOut AsValid<TOut>(
        this object? obj,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(obj))]
        string? valueName = null)
    {
        if (obj is TOut output)
            return output;

        throw new ArgumentException(
            exceptionMessage ?? $"The given {obj?.GetType().ToCode()} value is not a {typeof(TOut).ToCode()} instance",
            valueName);
    }
}