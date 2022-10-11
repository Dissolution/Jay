namespace Jay.Validation;

public static class Validate
{
    #region Replacement
    /// <summary>
    /// Replaces the given <see cref="string"/> with <see cref="string.Empty"/> if it is <see langword="null"/>.
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull([NotNull] ref string? value)
    {
        value ??= string.Empty;
    }

    /// <summary>
    /// Replaces the given <paramref name="value"/> if it is <see langword="null"/>
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="replacementIfNull"/> is <see langword="null"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull<T>([NotNull] ref T? value, [DisallowNull] T replacementIfNull)
    {
        if (value is null)
        {
            value = replacementIfNull ?? throw new ArgumentNullException(nameof(replacementIfNull));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull<T>([NotNull] ref T? value, Func<T> replacementValueFactory)
    {
        if (value is null)
        {
            value = replacementValueFactory() ?? 
                    throw new ArgumentException("The replacement value must not be null", nameof(replacementValueFactory));
        }
    }
    #endregion
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Index(int index, int length, [CallerArgumentExpression("index")] string? indexArgName = null)
    {
        if ((uint)index >= (uint)length)
        {
            throw new ArgumentOutOfRangeException(indexArgName, index, $"Index '{index}' must be between 0 and {length - 1}");
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Index(Index index, int length, [CallerArgumentExpression("index")] string? indexArgName = null)
    {
        int offset = index.GetOffset(length);
        if ((uint)offset >= (uint)length)
        {
            throw new ArgumentOutOfRangeException(indexArgName, index, $"Index '{index}' does not fit in length {length}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Insert(int index, int length, [CallerArgumentExpression("index")] string? indexArgName = null)
    {
        if ((uint)index > (uint)length)
        {
            throw new ArgumentOutOfRangeException(indexArgName, index, $"Insert index '{index}' must be between 0 and {length}");
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(int myLength, Array? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1)
            throw new ArgumentException("Array must have a rank of 1", nameof(array));
        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException("Array must have a lower bound of 0", nameof(array));
        if ((uint)arrayIndex > (uint)array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < myLength)
            throw new ArgumentException($"Array must have a capacity of at least {arrayIndex + myLength}", nameof(array));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(int myLength, T[]? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > (uint)array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < myLength)
            throw new ArgumentException($"Array must have at a capacity of at least {arrayIndex + myLength}", nameof(array));
    }
}