namespace Jay.Validation;

public static class ValidationExtensions
{
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