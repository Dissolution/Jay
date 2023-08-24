namespace Jay.Validation;

/// <summary>
/// Extensions related to Validation<br/>
/// These are all intended on being used fluently in-line, rather than called directly as with <see cref="Validate"/><br/>
/// e.g.:<br/>
/// <c>string name = collection.FirstOrDefault().ThrowIfNull().Name;</c>
/// </summary>
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
        [AllowNull] [NotNull] this T? value,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is not null) return value;
        throw new ArgumentNullException(valueName,
            exceptionMessage ?? $"The given {typeof(T).Name} value must not be null");
    }

    [return: NotNull]
    public static TOut AsValid<TOut>(this object? value,
        string? exceptionMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is TOut output) return output;
        throw new ArgumentException(
            exceptionMessage ?? $"The given {value?.GetType().Name} value is not a {typeof(TOut).Name} instance",
            valueName);
    }
}