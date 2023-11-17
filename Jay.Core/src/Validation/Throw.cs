namespace Jay.Validation;

public static class Throw
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <paramref name="value"/>
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> value to check for <c>null</c>
    /// </param>
    /// <param name="valueName">
    /// The name of the value argument, passed to a thrown <see cref="ArgumentNullException"/>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is <c>null</c>
    /// </exception>
    public static void IfNull<T>(
        [AllowNull, NotNull] T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        => Check.IfNull<T>(value, valueName).ThrowIfError();

    public static void IfNullOrEmpty(
        [AllowNull, NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
        => Check.IfNullOrEmpty(str, strName).ThrowIfError();

    public static void IfNullOrWhiteSpace(
        [AllowNull, NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
        => Check.IfNullOrWhiteSpace(str, strName).ThrowIfError();

    public static void IfNotIn<T>(
        T value,
        T inclusiveMinimum,
        T inclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        => Check.IfNotIn<T>(value, inclusiveMinimum, inclusiveMaximum).ThrowIfError();

    public static void Index(
        int available,
        int index,
        bool insert = false,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
        => Check.Index(available, index, insert, indexName).ThrowIfError();

    public static int Index(
        int available,
        Index index,
        bool insert = false,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
        => Check.Index(available, index, insert, indexName).OkValueOrThrowError();

    public static void Range(
        int available,
        int start,
        int length,
        [CallerArgumentExpression(nameof(start))]
        string? startName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
        => Check.Range(available, start, length, startName, lengthName).ThrowIfError();

    public static (int Start, int Length) Range(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
        => Check.Range(available, range, rangeName).OkValueOrThrowError();

    public static void CanCopyTo(
        int count,
        Array? array,
        int arrayIndex = 0,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
        => Check.CanCopyTo(count, array, arrayIndex, arrayName).ThrowIfError();

    public static void CanCopyTo<T>(
        int count,
        T[]? array,
        int arrayIndex = 0,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
        => Check.CanCopyTo<T>(count, array, arrayIndex, arrayName).ThrowIfError();
}