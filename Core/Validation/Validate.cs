namespace Jay.Validation;

/// <summary>
/// A utility class for all sort of Validation
/// </summary>
public static class Validate
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int offset, int length) FastResolve(int available, Range range)
    {
        int start;
        Index startIndex = range.Start;
        if (startIndex.IsFromEnd)
            start = available - startIndex.Value;
        else
            start = startIndex.Value;

        int end;
        Index endIndex = range.End;
        if (endIndex.IsFromEnd)
            end = available - endIndex.Value;
        else
            end = endIndex.Value;

        return (start, end - start);
    }


    /// <summary>
    /// Validates that <paramref name="value"/> is not <c>null</c>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/> to validate</typeparam>
    /// <param name="value">The <typeparamref name="T"/> value to check for <c>null</c></param>
    /// <param name="exMessage">An optional message for a thrown <see cref="ArgumentNullException"/></param>
    /// <param name="valueName">The name of the value argument</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is <c>null</c>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull<T>([AllowNull, NotNull] T value,
        string? exMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is null)
            throw new ArgumentNullException(valueName, exMessage);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNullOrEmpty([AllowNull, NotNull] string str,
        string? exMessage = null,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (!string.IsNullOrEmpty(str)) return;
        throw new ArgumentException(
            exMessage,
            strName);
    }


    public static void Between<T>(T value, 
        T minInc, 
        T maxInc,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : IComparable<T>
    {
        int c = value.CompareTo(minInc);
        if (c < 0)
            throw new ArgumentOutOfRangeException(valueName, value, $"Value must be greater than or equal to {minInc}");
        c = value.CompareTo(maxInc);
        if (c > 0)
            throw new ArgumentOutOfRangeException(valueName, value, $"Value must be lesser than or equal to {maxInc}");
    }
    

    /// <summary>
    /// Validates that the <paramref name="index"/> fits in <paramref name="available"/>
    /// </summary>
    /// <param name="available">The number of items available to be indexed into</param>
    /// <param name="index">The index attempting to be referenced</param>
    /// <param name="indexName">The name of the index parameter</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> <c>is</c> &lt; 0 <c>or</c> &gt;= <paramref name="available"/>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Index(int available, int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index < (uint)available) return;
        throw new ArgumentOutOfRangeException(
            indexName,
            index,
            $"{indexName} must be between 0 and {available - 1}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexGetLength(
        int available, 
        int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index < (uint)available) return available - index;
        throw new ArgumentOutOfRangeException(
            indexName,
            index,
            $"{indexName} must be between 0 and {available - 1}");
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Index(int available, Index index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);
        if ((uint)offset < available) return;
        throw new ArgumentOutOfRangeException(indexName,
            index,
            $"{indexName} {index} must be between 0 and {available - 1}");
    }

    public static void StartLength(
        int available,
        int start,
        int length,
        [CallerArgumentExpression(nameof(start))]
        string? startName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        if ((uint)start > (uint)available)
            throw new ArgumentOutOfRangeException(startName, start, $"Starting Index must be between 0 and {available}");
        if ((uint)length > (uint)(available - start))
            throw new ArgumentOutOfRangeException(lengthName, length, $"Length must be between 0 and {available - start}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Range(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        (int offset, int length) = FastResolve(available, range);
        if ((uint)offset + (uint)length <= (uint)available) return;
        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"{rangeName} must be between 0 and {available - 1}");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int offset, int length) RangeResolve(
        int available,
        Range range,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        var ol = FastResolve(available, range);
        if ((uint)ol.offset + (uint)ol.length <= (uint)available) return ol;
        throw new ArgumentOutOfRangeException(
            rangeName,
            range,
            $"{rangeName} must be between 0 and {available - 1}");
    }


    public static void InsertIndex(int available, int index,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index <= available) return;
        throw new ArgumentOutOfRangeException(indexName,
            index,
            $"Insert {indexName} {index} must be between 0 and {available}");
    }
  

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(int available, Array? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if (array.Rank != 1)
            throw new ArgumentException("Array must have a rank of 1", nameof(array));
        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException("Array must have a lower bound of 0", nameof(array));
        if ((uint)arrayIndex > array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < available)
            throw new ArgumentException($"Array must have a capacity of at least {arrayIndex + available}", nameof(array));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(int available, T[]? array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > array.Length)
            throw new IndexOutOfRangeException($"Array Index '{arrayIndex}' must be between 0 and {array.Length - 1}");
        if (array.Length - arrayIndex < available)
            throw new ArgumentException($"Array must have at a capacity of at least {arrayIndex + available}", nameof(array));
    }


#region Replacement
    /// <summary>
    /// Replaces the given <see cref="string"/> with <see cref="string.Empty"/> if it is <see langword="null"/>.
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull([AllowNull, NotNull] ref string? value)
    {
        value ??= string.Empty;
    }

    /// <summary>
    /// Replaces the given <paramref name="value"/> if it is <see langword="null"/>
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="replacementIfNull"/> is <see langword="null"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull<T>([AllowNull, NotNull] ref T? value, [DisallowNull] T replacementIfNull)
    {
        if (value is null)
        {
            value = replacementIfNull ?? throw new ArgumentNullException(nameof(replacementIfNull));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReplaceIfNull<T>([AllowNull, NotNull] ref T? value, Func<T> replacementValueFactory)
    {
        if (value is null)
        {
            value = replacementValueFactory() ??
                    throw new ArgumentException("The replacement value must not be null", nameof(replacementValueFactory));
        }
    }
#endregion
}